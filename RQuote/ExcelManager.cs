using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RQuote
{
    class ExcelManager
    {
        private const int imageHeight = 70;
        private const int rowHeight = 100;
        public ExcelManager()
        {

        }

        public List<string> GetVendorCatalogParentFolderIds(string mainFilePath, string vendorId)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mainFilePath)))
            {
                var catalogWorkSheet = package.Workbook.Worksheets[1];
                var catalogIds = new List<string>();
                var totalColumns = catalogWorkSheet.Dimension.End.Column;
                var x = (from cell in catalogWorkSheet.Cells["A:A"] where cell.Text != null && cell.Text == vendorId select cell);
                if(x.ToArray().Length > 0)
                {
                    var vendorRow = x.First().End;
                    var parentFolderIds = from cell in catalogWorkSheet.Cells[vendorRow.Row,vendorRow.Column+2,vendorRow.Row,totalColumns] where cell.Text != null select cell.Text;
                    return parentFolderIds.ToList();
                }
            }
            return null;
        }

        public string GetVendorTemplateId(string mainFilePath, string vendorId)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(mainFilePath)))
            {
                var catalogWorkSheet = package.Workbook.Worksheets[1];
                var catalogIds = new List<string>();
                var totalColumns = catalogWorkSheet.Dimension.End.Column;
                var x = (from cell in catalogWorkSheet.Cells["A:A"] where cell.Text != null && cell.Text == vendorId select cell);
                if (x.ToArray().Length > 0)
                {
                    var vendorRow = x.First().End;
                    var templateId = from cell in catalogWorkSheet.Cells[vendorRow.Row, vendorRow.Column + 1, vendorRow.Row, vendorRow.Column + 1] where cell.Text != null select cell.Text;
                    return templateId.FirstOrDefault();
                }
            }
            return null;
        }

        public List<Product> LoadProductsForCatalog(Catalog catalog)
        {
            List<Product> productList = new List<Product>();
            try
            {
                using (var package = new ExcelPackage(new FileInfo(catalog.FilePath)))
                {
                    var catalogWorkSheet = package.Workbook.Worksheets.First();
                    var totalRows = catalogWorkSheet.Dimension.End.Row;
                    //Model No: B : 2
                    //Details : V : 22
                    for(int rowNum = 2; rowNum <= totalRows; rowNum++)
                    {
                        //[int FromRow, int FromCol, int ToRow, int ToCol]
                        var modelNo = (catalogWorkSheet.Cells[rowNum, 2, rowNum, 2].Select(c => c.Value == null ? string.Empty : c.Value.ToString())).FirstOrDefault();
                        var desc = (catalogWorkSheet.Cells[rowNum, 22, rowNum, 22].Select(c => c.Value == null ? string.Empty : c.Value.ToString())).FirstOrDefault();
                        var price = (catalogWorkSheet.Cells[rowNum, 15, rowNum, 15].Select(c => c.Value == null ? string.Empty : c.Value.ToString())).FirstOrDefault();
                        double priceNumber = 0.0;
                        if (!String.IsNullOrEmpty(modelNo))
                        {
                            desc = desc.Trim();
                            try
                            {
                                priceNumber = double.Parse(price);
                            }
                            catch (Exception e)
                            {

                            }
                            productList.Add(new Product(catalog) { ModelNo = modelNo, Details = desc , Price = priceNumber});
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return productList;
        }


        public void SaveQuotation(QuotationPageDataContext quotationDetail, string templateFilePath, string destinationPath)
        {
            try
            {
                using (var package = new ExcelPackage(new FileInfo(templateFilePath)))
                {
                    var referenceWorksheet = package.Workbook.Worksheets["cellReference"];
                    var quoteWs = package.Workbook.Worksheets[0];
                    if(referenceWorksheet != null)
                    {
                        var firstName = referenceWorksheet.Cells["B1"].Value.ToString();
                        var lastName = referenceWorksheet.Cells["B2"].Value.ToString();
                        var email = referenceWorksheet.Cells["B3"].Value.ToString();
                        var phone = referenceWorksheet.Cells["B4"].Value.ToString();
                        var orgName = referenceWorksheet.Cells["B5"].Value.ToString();
                        var projName = referenceWorksheet.Cells["B6"].Value.ToString();
                        var quotNo = referenceWorksheet.Cells["B7"].Value.ToString();
                        var quotDate = referenceWorksheet.Cells["B8"].Value.ToString();
                        var lineStart = referenceWorksheet.Cells["B9"].Value.ToString();

                        quoteWs.Cells[firstName].Value = quotationDetail.CustomerDetails.FirstName;
                        quoteWs.Cells[lastName].Value = quotationDetail.CustomerDetails.LastName;
                        quoteWs.Cells[email].Value = quotationDetail.CustomerDetails.Email;
                        quoteWs.Cells[phone].Value = quotationDetail.CustomerDetails.Phone;
                        quoteWs.Cells[orgName].Value = quotationDetail.CustomerDetails.OrganizationName;
                        quoteWs.Cells[projName].Value = quotationDetail.CustomerDetails.ProjectName;
                        quoteWs.Cells[quotNo].Value = quotationDetail.QuotationNumber;
                        quoteWs.Cells[quotDate].Value = quotationDetail.QuotationDate;

                        var lineStartCell = quoteWs.Cells[lineStart];
                        int startRow = lineStartCell.Start.Row;
                        int startCol = lineStartCell.Start.Column;
                        int slNo = 1;
                        foreach(var lineItem in quotationDetail.QuoteLines)
                        {
                            quoteWs.Row(startRow).Height = rowHeight;

                            quoteWs.Cells[startRow, startCol].Value = slNo++;
                            quoteWs.Cells[startRow, startCol + 1].Value = lineItem.PartNo;

                            AddImage(quoteWs, startRow, startCol + 2, lineItem.PartNo, lineItem.Image);

                            quoteWs.Cells[startRow, startCol + 3].Value = lineItem.ProductDetails;
                            quoteWs.Cells[startRow, startCol + 3].Style.WrapText = true;
                            quoteWs.Cells[startRow, startCol + 4].Value = lineItem.Unit;
                            quoteWs.Cells[startRow, startCol + 5].Value = lineItem.Quantity;
                            quoteWs.Cells[startRow, startCol + 6].Value = lineItem.Price;
                            quoteWs.Cells[startRow, startCol + 7].Value = lineItem.Discount;
                            quoteWs.Cells[startRow, startCol + 8].Value = lineItem.Amount;
                            quoteWs.Cells[startRow, startCol + 9].Value = lineItem.SelectedGST;
                            quoteWs.Cells[startRow, startCol + 10].Value = lineItem.GSTAmount;
                            quoteWs.Cells[startRow, startCol + 11].Value = lineItem.Total;

                            startRow++;
                        }

                        package.Workbook.Worksheets.Delete(referenceWorksheet);
                        package.SaveAs(new FileInfo(destinationPath));
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void AddImage(ExcelWorksheet oSheet, int rowIndex, int colIndex, string imageName,string imagePath)
        {
            Bitmap image = File.Exists(imagePath) ? new Bitmap(imagePath) : null;
            ExcelPicture excelImage = null;
            if (image != null)
            {
                excelImage = oSheet.Drawings.AddPicture(imageName, image);
                excelImage.From.Column = colIndex - 1;
                excelImage.From.Row = rowIndex - 1;
                excelImage.SetSize(imageHeight, imageHeight);
                // 2x2 px space for better alignment
                excelImage.From.ColumnOff = Pixel2MTU(2);
                excelImage.From.RowOff = Pixel2MTU(2);
            }
        }

        public int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }
    }
}
