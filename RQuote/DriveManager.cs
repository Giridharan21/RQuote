using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RQuote
{
    class DriveManager
    {
        static string[] Scopes = { DriveService.Scope.DriveReadonly };
        static string ApplicationName = "RQuote";
        DriveService service;
        public LandingPageDataContext LandingPageDataContext;
        public DriveManager()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credential/credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "authToken.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            //ServiceAccountCredential serviceAccount = new ServiceAccountCredential()
            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                //ApiKey = "AIzaSyASY0vjXN2opV4sRW7Lr-vRhrLJGt3Pn0s",
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public string DownloadInitialFile()
        {
            string fileName = Path.Combine(Utils.AppDataPath, "VendorDetails") + ".xlsx";
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                String fileId = "1v6xVu78A3j5e_qMaWGcq0PgYkpputiQ9ZMWII1Tl4WA";
                IDownloadProgress progress = new FilesResource.ExportRequest(service, fileId, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").DownloadWithStatus(fs);
                return progress.Status == DownloadStatus.Completed ? fileName : null;
            }
        }

        public List<Task> DownloadFolderContents(string folderId)
        {
            List<Task> downloadTasks = new List<Task>();
            String cataloguePath = Utils.CataloguesFolderPath;
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "files(*)";
            listRequest.Q = "'" + folderId + "' in parents and trashed=false";
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;
            var excelFile = from file in files where file.MimeType == "application/vnd.google-apps.spreadsheet" select file;
            var imageFolder = from file in files where file.MimeType == "application/vnd.google-apps.folder" select file;
            //Download the Excel File
            if (excelFile.Any())
            {
                try
                {
                    if (!Directory.Exists(cataloguePath + "\\" + excelFile.First().Name))
                    {
                        Utils.CreateDirectory(cataloguePath + "\\" + excelFile.First().Name);
                        Utils.CreateDirectory(cataloguePath + "\\" + excelFile.First().Name + "\\images");
                    }
                    Task t = downloadExcel(excelFile.First().Id, cataloguePath + "\\" + excelFile.First().Name + "\\" + excelFile.First().Name + ".xlsx");
                    downloadTasks.Add(t);
                    t.Start();
                    //Download Images from folder
                    if (imageFolder.Any())
                    {
                       downloadTasks.AddRange(downloadImagesFolder(imageFolder.First().Id, cataloguePath + "\\" + excelFile.First().Name + "\\images"));
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return downloadTasks;
        }

        public Task DownloadFile(string fileId, string filePath)
        {
            IncreaseDownloadCount();
            return new Task(() =>
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        var progress = new FilesResource.GetRequest(service, fileId).DownloadWithStatus(fs);
                        if (progress.Status == DownloadStatus.Completed)
                        {
                            IncreaseDownloadedCount();
                        }
                    }
                }
                catch(Exception ex)
                {
                    LogUtil.Logger.Error(ex);
                    IncreaseDownloadedCount();
                }
            });
        }


        private Task downloadExcel(string fileId, string filePath)
        {
            IncreaseDownloadCount();
            return new Task(() =>
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    IDownloadProgress progress =  new FilesResource.ExportRequest(service, fileId, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").DownloadWithStatus(fs);
                    if(progress.Status == DownloadStatus.Completed)
                    {
                        IncreaseDownloadedCount();
                    }
                    //await new FilesResource.ExportRequest(service, fileId, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").DownloadAsync(fs);
                }
            });
        }

        private List<Task> downloadImagesFolder(string folderId, string folderPath)
        {
            List<Task> list = new List<Task>();
            List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageToken = pageToken;
                listRequest.PageSize = 10;
                listRequest.Fields = "nextPageToken, files(*)";
                listRequest.Q = "'" + folderId + "' in parents and trashed=false and mimeType contains 'image/'";
                var result = listRequest.Execute();
                files.AddRange(result.Files);
                IncreaseDownloadCount(result.Files.Count);
                foreach (var file in result.Files)
                {
                    string imageDownloadPath = folderPath + "\\" + file.Name;
                    bool shouldDownloadFile = true;
                    if(File.Exists(imageDownloadPath))
                    {
                        //Check if the new file has been modified
                        DateTime dt1 = File.GetLastWriteTime(imageDownloadPath);
                        DateTime dt2 = file.ModifiedTime.HasValue ? file.ModifiedTime.Value : DateTime.MinValue;

                        if(dt2 < dt1)
                        {
                            shouldDownloadFile = false;
                            IncreaseDownloadedCount();
                        }
                    }
                    if(shouldDownloadFile)
                    {
                        Task t = downloadImage(file.Id, imageDownloadPath);
                        list.Add(t);
                        t.Start();
                    }
                }
                pageToken = result.NextPageToken;
                //pageToken = null;
            } while (pageToken != null);
            
            return list;
        }

        private Task downloadImage(string fileId, string filePath)
        {
            //IncreaseDownloadCount();
            return new Task(() =>
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    var progress = new FilesResource.GetRequest(service, fileId).DownloadWithStatus(fs);
                    if (progress.Status == DownloadStatus.Completed)
                    {
                        IncreaseDownloadedCount();
                    }
                }
            });
            //using (FileStream fs = new FileStream(filePath, FileMode.Create))
            //{
            //    Task<IDownloadProgress> progress = new FilesResource.GetRequest(service, fileId).DownloadAsync(fs);
            //    if (progress.Result.Status == DownloadStatus.Completed)
            //    {
            //        IncreaseDownloadedCount();
            //    }
            //    return progress;
            //}
        }

        private void IncreaseDownloadedCount()
        {
            if (LandingPageDataContext != null)
            {
                LandingPageDataContext.DownloadedFiles++;
            }
        }

        private void IncreaseDownloadCount(int num = -1)
        {
            if (LandingPageDataContext != null)
            {
                if(num == -1)
                {
                    LandingPageDataContext.TotalFiles++;
                }
                else
                {
                    LandingPageDataContext.TotalFiles += num;
                }
            }
        }
    }
}
