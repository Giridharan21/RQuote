using ClosedXML.Excel;
using RQuote.Server;
using RQuote.Server.Tables;


//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQuote.Logic
{
    public class ExcelOperations
    {
        public string FilePath { get; set; }
        
        public Dictionary<string,string> ColumnNames { get; set; }

        public ExcelOperations(string filePath)
        {
            FilePath = filePath;
            ColumnNames = new Dictionary<string, string>();
        }

        public List<string> GetHeaders()
        {
            var list = new List<string>();

            using (var WorkBook = new XLWorkbook(FilePath))
            {
                IXLWorksheet Worksheet = WorkBook.Worksheet(1);
                var header = Worksheet.RowsUsed().FirstOrDefault();
                foreach (var cell in header.CellsUsed())
                {
                    var str = string.Join("", cell.Value.ToString().Where(i => i != ' ' && i != '/' && i != '-' && i != '.'));
                    list.Add(str);
                    //Console.WriteLine(cell.WorksheetColumn().ColumnNumber());
                    ColumnNames.Add(cell.WorksheetColumn().ColumnLetter(), str);
                }
            }
            return list;
        }
        public List<Products> GetProducts()
        {
            var products = new List<Products>();
            using (var WorkBook = new XLWorkbook(FilePath))
            {
                IXLWorksheet Worksheet = WorkBook.Worksheet(1);
                var rows = Worksheet.RowsUsed().Skip(4611);
                Console.WriteLine(rows.Count()); 
                int nu = 0;
                foreach (var row in rows)
                {
                    //Console.WriteLine(++nu);
                    var product = new Products();
                    try
                    {
                        string model = row.Cell("C")?.Value.ToString()??"".Trim();
                        string catalogue = row.Cell("AT")?.Value.ToString()??"100";
                        double.TryParse(row.Cell("P")?.Value.ToString(), out double price);
                        product.ModelNo = model.Trim();
                        product.CatalogueId = Convert.ToInt32(catalogue);
                        product.Price = price;
                        var details = row.CellsUsed().Count()==0?new List<string>(): row.CellsUsed().Select(i => ColumnNames[i.WorksheetColumn().ColumnLetter()] + " : " + i.Value.ToString());
                        product.Details = details.Count()==0?"": string.Join("; ",details);
                        product.Image = new byte[0];
                        product.CreatedById = 3;
                        product.Uploaded = true;
                        if (model != "" && catalogue != "")
                            product.Image = ImageAsByteArr(string.Join("",model.Trim().Where(i => i != '!')), catalogue);
                        if (products.Select(i => i.ModelNo).Contains(model))
                            product.ModelNo += "!";
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    //foreach (var cell in row.CellsUsed())
                    //{
                        

                    //}
                }
            }
            return products;
        }
        public async Task AddProducts(List<Products> products)
        {
            var context = new ServerDbContext();
            context.Configuration.AutoDetectChangesEnabled = false;
            int count = 0;
            foreach (var x in products)
            {
                count++;
                context.Set<Products>().Add(x);
                if(count%10==0)
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine(count);
                }
                if(count % 50 == 0)
                {
                    context = new ServerDbContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }
            
            await context.SaveChangesAsync();
        }
        public List<string> GetProductDetails()
        {
            var list = new List<string>();

            using (var WorkBook = new XLWorkbook(FilePath))
            {
                IXLWorksheet Worksheet = WorkBook.Worksheet(1);
                var rows = Worksheet.RowsUsed().Skip(1);
                foreach (var row in rows)
                {
                    try
                    {
                        var details = row.CellsUsed().Count() == 0 ? new List<string>() : row.CellsUsed().Select(i => ColumnNames[i.WorksheetColumn().ColumnLetter()] + " : " + i.Value.ToString());
                        var str = details.Count() == 0 ? "" : string.Join("; ", details);
                        list.Add(str);
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    //foreach (var cell in row.CellsUsed())
                    //{


                    //}
                }
            }


            return list;
        }
        public byte[] ImageAsByteArr(string name,string CatalogueId)
        {
            byte[] byteArr = new byte[0];
            
            string path = "";

            switch (CatalogueId)
            {
                case "1":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\Hybec\Ecobrite\images\" + name +".jpg";
                    if(File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;
                case "2":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\Hybec\Elite\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;
                case "3":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\Klite\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;

                case "4":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\Ledlum\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} " );
                    break;

                case "5":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\LEDwell\LEDwell\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;

                case "6":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\LEDwell\LEDwellNXT\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;

                case "7":
                    path = @"C:\Users\844617\Downloads\drive-download-20201122T113452Z-001\LNS\images\" + name + ".jpg";
                    if (File.Exists(path))
                        byteArr = File.ReadAllBytes(path);
                    else
                        Console.WriteLine($"No File  {name} catalog {CatalogueId} ");

                    break;


                default:
                    break;
            }
            return byteArr;
        }
        public class ShowroomModel
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Address { get; set; }
            public string BankDetails { get; set; }
            public string TermsAndConditions { get; set; }
            public byte[] ShowroomImage { get; set; }
            public List<int> Catalogues { get; set; }
            public List<int> Users { get; set; }
        }
        public void AddShowRoom(ShowroomModel model)
        {
            var context = new ServerDbContext();
            var showroom = new Showrooms
            {
                Address = model.Address,
                BankDetails = model.BankDetails,
                Catalogues = context.Catalogues.Where(i => model.Catalogues.Contains(i.Id)).ToList(),
                Code = model.Code,
                Name = model.Name,
                ShowroomImage = model.ShowroomImage,
                TermsAndConditions = model.TermsAndConditions,
                Users = context.Users.Where(i => model.Users.Contains(i.Id)).ToList(),
            };
            context.Showrooms.Add(showroom);
            context.SaveChanges();
        }
        public List<string> GetRowsValues()
        {
            var list = new List<string>();
            var cols = new List<string>();
            var values = new List<string>();
            using (var WorkBook = new XLWorkbook(FilePath))
            {
                IXLWorksheet Worksheet = WorkBook.Worksheet(1);
                var rows = Worksheet.RowsUsed().Skip(1).Take(10);
                int i = 0;
                Console.WriteLine(rows.Count());
                foreach (var row in rows)
                {
                    foreach (var cell in row.CellsUsed())
                    {
                        try
                        {
                            //string str = $"{cell.Value} pos {cell.WorksheetColumn().ColumnLetter()}";
                            //list.Add(str);
                            cols.Add(ColumnNames[cell.WorksheetColumn().ColumnLetter()]);
                            values.Add(cell.Value.ToString());
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine(cell.WorksheetColumn().ColumnLetter() + "  "+ x.Message);
                        }
                    }
                    var s1 = string.Join(",", cols);
                    var s2 = string.Join(",", values);
                    //list.Add($"Insert into Product(ModelNo,Details,Price,CatalogueId) values({i},{s2})");
                    //list.Add($"Insert into ProductProperties(Id,{s1}) values({i},{s2})");
                }
            }

            return list;


        }
        //public List<List<string>> GetRows()
        //{
        //    var rows = new List<List<string>>();
        //    using (var spreadsheet = SpreadsheetDocument.Open(FilePath,false))
        //    {
        //        WorkbookPart workbookPart = spreadsheet.WorkbookPart;
        //        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        //        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
        //        int i = 0;
        //        foreach (Row r in sheetData.Elements<Row>().Take(11))
        //        {
        //            i++;
        //            int j = 0;
        //            Console.WriteLine($"Row {i} -  {r.Elements<Cell>().Count()}");
        //            foreach (Cell c in r.Elements<Cell>())
        //            {
                        
        //                string cellvalue = "";
        //                j++;
        //                if(c is null)
        //                {
        //                    Console.WriteLine($"null at {i} {j}");
        //                }
        //                else
        //                {
        //                    if(c.DataType!=null && c.DataType == CellValues.SharedString)
        //                    {
        //                        if(int.TryParse(c.CellValue.InnerText,out int id))
        //                        {
        //                            cellvalue =workbookPart.SharedStringTablePart.
        //                                SharedStringTable.Elements<SharedStringItem>().ElementAt(id).InnerText;
        //                        }
        //                    }
        //                    else if(c.CellValue != null)
        //                    {
        //                        cellvalue = c.InnerText;
        //                    }
        //                    else
        //                    {
        //                        cellvalue = $"Empty cell value {i} {j}";
        //                    }
        //                    Console.WriteLine(cellvalue);
        //                }
                        
        //            }
        //        }
        //    }
        //    return rows;
        //}
    }
}
