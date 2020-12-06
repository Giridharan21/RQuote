using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Security.AccessControl;
using System.Security.Principal;
using NLog;

namespace RQuote
{
    public static class LogUtil
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = Utils.AppDataPath + "\\logfile.txt" };
            
            // Rules for mapping loggers to targets            
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            //config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            //config.AddRule(LogLevel.Error, LogLevel.Fatal, logfile);
            config.AddRuleForAllLevels(logfile);
            // Apply config           
            NLog.LogManager.Configuration = config;
        }
    }
    public static class Utils
    {
        private static string EncryptionKey = "sblw-3hn8-sqoy19";
        private static string AppFolderName = "RQuote";
        private static string AppInitializedFileName = "initialized.dat";
        private static string TemplateFileName = "template.html";
        private static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string AppDataPath = LocalAppData + "\\" + AppFolderName;
        public static string SavedQuotationsPath = LocalAppData + "\\" + AppFolderName + "\\" + "SavedQuotations";
        public static string CataloguesFolderPath = LocalAppData + "\\" + AppFolderName + "\\" + "Catalogues";
        public static string CustomProductsFolderPath = LocalAppData + "\\" + AppFolderName + "\\Custom";
        public static string CustomProductsImageFolderPath = LocalAppData + "\\" + AppFolderName + "\\Custom\\Images";
        public static string CustomProductsFilePath = LocalAppData + "\\" + AppFolderName + "\\Custom\\" + "CustomProducts.bin";
        public static string TemplateFilePath = LocalAppData + "\\" + AppFolderName + "\\" + TemplateFileName;

        public static bool IsApplicationInitialized()
        {
            bool areFilesPresent = Directory.Exists(LocalAppData + "\\" + AppFolderName)
                && File.Exists(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName);
            bool isSetupDone = false;
            if(areFilesPresent)
            {
                string fileContent = File.ReadAllText(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName);
                isSetupDone = fileContent.Trim().Length > 0;
            }
            return areFilesPresent && isSetupDone;
        }

        public static bool InitializeAppFolder()
        {
            bool created = false;

            try
            {
                if (!Directory.Exists(LocalAppData + "\\" + AppFolderName))
                {
                    CreateDirectory(LocalAppData + "\\" + AppFolderName);
                }
                if (!Directory.Exists(LocalAppData + "\\" + AppFolderName + "\\" + "Catalogues"))
                {
                    CreateDirectory(LocalAppData + "\\" + AppFolderName + "\\" + "Catalogues");
                }
                if (!File.Exists(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName))
                {
                    File.Create(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName);
                   // File.SetAttributes(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName, File.GetAttributes(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName) | FileAttributes.Hidden);
                }
                if(!Directory.Exists(CustomProductsFolderPath))
                {
                    CreateDirectory(CustomProductsFolderPath);
                }
                if (!Directory.Exists(CustomProductsImageFolderPath))
                {
                    CreateDirectory(CustomProductsImageFolderPath);
                }
                created = true;
                //created = IsApplicationInitialized();
            }
            catch (Exception ex)
            {

            }

            return created;
        }

        public static void PersistCustomer(string customerId)
        {
            try
            {
                using (FileStream fs = new FileStream(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName, FileMode.OpenOrCreate))
                {
                    byte[] b = Encoding.ASCII.GetBytes(customerId);
                    fs.Write(b, 0, b.Length);
                    fs.Flush();
                    fs.Close();
                }
                //File.WriteAllText(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName, customerId);
            }
            catch(Exception ex)
            {
                LogUtil.Logger.Error(ex);
            }
        }

        public static string GetCurrentCustomerId()
        {
            return File.Exists(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName) ? File.ReadAllText(LocalAppData + "\\" + AppFolderName + "\\" + AppInitializedFileName) : null;
        }

        public static int GetQuoteNumber()
        {
            int quoteNum = 1;
            try
            {
                string fName = Path.Combine(AppDataPath, "quoteNum.dat");
                if (!File.Exists(fName))
                {
                    File.Create(fName);
                }
                string text = File.ReadAllText(fName);
                var lastModified = System.IO.File.GetLastWriteTime(fName);
                if (String.IsNullOrEmpty(text) || lastModified.Date != DateTime.Now.Date)
                {
                    File.WriteAllText(fName, "0");
                }
                else
                {

                    int lastNum = int.Parse(text);
                    quoteNum = lastNum + 1;
                }
            }
            catch (Exception ex)
            {

            }
            return quoteNum;
        }

        public static void IncreaseQuoteNumber()
        {
            try
            {
                int quoteNum = 1;
                string fName = Path.Combine(AppDataPath, "quoteNum.dat");
                if (!File.Exists(fName))
                {
                    File.Create(fName);
                }
                string text = File.ReadAllText(fName);
                var lastModified = System.IO.File.GetLastWriteTime(fName);
                if (String.IsNullOrEmpty(text) || lastModified.Date != DateTime.Now.Date)
                {
                    File.WriteAllText(fName, "0");
                }
                else
                {

                    int lastNum = int.Parse(text);
                    quoteNum = lastNum + 1;
                }
                File.WriteAllText(fName, quoteNum.ToString());
            }
            catch (Exception ex)
            {

            }
        }

        public static string Encrypt(string input)
        {
            string key = EncryptionKey;
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input)
        {
            string key = EncryptionKey;
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static void CreateDirectory(string dirPath)
        {
            if(!Directory.Exists(dirPath))
            {
                DirectorySecurity securityRules = new DirectorySecurity();
                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                //FileSystemAccessRule fsRule = new FileSystemAccessRule(sid, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow);
                //securityRules.AddAccessRule(fsRule);
                securityRules.AddAccessRule(new FileSystemAccessRule(sid, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                securityRules.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.CreatorOwnerSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                DirectoryInfo di = Directory.CreateDirectory(dirPath, securityRules);
                //di.SetAccessControl(securityRules);
                //di.Attributes |= FileAttributes.ReadOnly;
            }
        }
    }
}
