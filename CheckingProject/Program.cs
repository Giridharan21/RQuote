using RQuote.Logic;
using RQuote.Server.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace giri
{

}
namespace CheckingProject
{
    class Program
    {
        static bool CheckExists(List<string> list, string value)
        {
            return (list.Where(i => i == value).Count() > 1);
        }
        static async Task Main(string[] args)
        {

            //SqlConnection con = new SqlConnection(@"Data Source=SQL5086.site4now.net;Initial Catalog=DB_A6B5D4_RQuoteServer2;User Id=DB_A6B5D4_RQuoteServer2_admin;Password=RQuote123!;");
            //SqlDataAdapter adapter = new SqlDataAdapter("select * from Catalogues", con);
            //DataSet dt = new DataSet();
            //adapter.Fill(dt);
            //var table = dt.Tables[0];


            var path = @"C:\Users\844617\Downloads\lightCatalogues-7_nav.xlsx";
            var excel = new ExcelOperations(path);

            
            var rows = new List<string>();
            var headers = excel.GetHeaders();

            var products = excel.GetProducts();
            //InsertProducts(products);
            var ModelNo = products.Select(i => i.ModelNo);
            var temp = products.Where(i => products.Where(j => j.ModelNo == i.ModelNo).Count() > 1);
            foreach (var x in temp)
            {
                //Console.WriteLine($"{x.ModelNo} {x.CatalogueId} ");
            }

            //for (int i = 0; i < products.Count; i++)
            //{
            //    if(ModelNo.Where(j=>j==products[i].ModelNo).Count() > 1)
            //    {
            //        products[i].ModelNo += "!";
            //    }
            //    //if(CheckExists(products.Select(j=>j.ModelNo).Take(i+1).ToList(),products[i].ModelNo))
            //    //    Console.WriteLine(products[i].ModelNo);
            //}
            try
            {
                //await excel.AddProducts(products);
            }
            catch (Exception exp)
            {

                throw exp; 
            }
            //var temp = products.Where(i => products.Where(j => j.ModelNo == i.ModelNo).Count() > 1);
            //Console.WriteLine(temp.Count());

           
            //foreach (var item in headers)
            //{
            //    //Console.WriteLine(item);
            //}
            Console.WriteLine("Finished");
            int count = 0;
            var ids = File.ReadAllLines(@"C:\Users\844617\Downloads\id.txt").Select(i=>int.Parse(i)).ToList();
            var cmds = new List<string>();
            var details = excel.GetProductDetails();
            foreach (var item in details)
            {

                var cols = new List<string>();
                var val = new List<string>();
                var detail = item.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var x in detail)
                {
                    var y = x.Split(new string[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                    if (y.Length == 2)
                    {
                        if (y[0] == "catalogueId")
                            continue;
                        cols.Add(y[0]);
                        val.Add(y[1].Replace("'", "''"));
                    }

                }
                var str = $"Insert into ProductProperties(ProductId,[{string.Join("],[", cols)}]) values({ids[count++]},'{string.Join("','", val)}');";
                cmds.Add(str);
                //Console.WriteLine(str);
                //Console.ReadLine();
                //context.Database.ExecuteSqlCommand
            }
            File.WriteAllLines(@"C:\Users\844617\Downloads\Queries.txt", cmds);
            //context.SaveChanges();
            //Console.WriteLine($"finished");
            /// Console.ReadLine();
            ///
            //foreach (var x in excel.GetHeaders())
            //{
            //    var y = "public string " + string.Join("", x.Where(i => i != ' ' && i != '/' && i != '-' && i != '.')) + " { get; set; }";
            //    //Console.WriteLine(y);
            //    output += string.Join("", x.Where(i => i != ' ' && i != '/' && i != '-' && i != '.')) + "\t";
            //}
            //rows.Add(output);
            //foreach (var x in excel.GetRowsValues().Take(10))
            //{
            //    Console.WriteLine(x);
            //    Console.WriteLine();
            //    rows.Add(string.Join("\t", x));
            //}
            //File.WriteAllLines(@"C:\Users\844617\Downloads\check.txt", rows);


        }
        static void InsertProducts(IEnumerable<Products> products)
        {
            using (SqlConnection oConnection = new SqlConnection(@"Data Source=SQL5086.site4now.net;Initial Catalog=DB_A6B5D4_RQuoteServer2;User Id=DB_A6B5D4_RQuoteServer2_admin;Password=RQuote123!;"))
            {
                oConnection.Open();
                using (SqlTransaction oTransaction = oConnection.BeginTransaction())
                {
                    using (SqlCommand oCommand = oConnection.CreateCommand())
                    {
                        
                        oCommand.Transaction = oTransaction;
                        oCommand.CommandType = CommandType.Text;
                        oCommand.CommandText = "SET IDENTITY_INSERT [dbo].[Products] ON";
                        oCommand.ExecuteNonQuery();
                        oTransaction.Commit();
                        oCommand.CommandText = "INSERT INTO [dbo].[Products] ([Id],[Details], [Price], [CatalogueId]" +
                            ", [ModelNo], [Image], [Downloaded]) VALUES (@id,@details,@price,@cid,@model,@image, @down)";
                        oCommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                        oCommand.Parameters.Add(new SqlParameter("@details", SqlDbType.NVarChar));
                        oCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.NVarChar));
                        oCommand.Parameters.Add(new SqlParameter("@cid", SqlDbType.NVarChar));
                        oCommand.Parameters.Add(new SqlParameter("@model", SqlDbType.NVarChar));
                        oCommand.Parameters.Add(new SqlParameter("@image", SqlDbType.VarBinary));
                        oCommand.Parameters.Add(new SqlParameter("@down", SqlDbType.Bit));
                        oCommand.Parameters[6].Value = 1;


                        try
                        {
                           
                            int i = 5000;
                            foreach (var x in products)
                            {
                                oCommand.Parameters[0].Value = i++;
                                oCommand.Parameters[1].Value = x.Details;
                                oCommand.Parameters[2].Value = x.Price;
                                oCommand.Parameters[3].Value = x.CatalogueId;
                                oCommand.Parameters[4].Value = x.ModelNo;
                                oCommand.Parameters[5].Value = x.Image;
                                if (oCommand.ExecuteNonQuery() != 1)
                                {
                                    throw new InvalidProgramException();
                                }
                                Console.WriteLine(i);
                            }
                            oCommand.CommandText = "SET IDENTITY_INSERT [dbo].[Products] OFF";
                            oCommand.ExecuteNonQuery();
                            oTransaction.Commit();

                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("an error " + e.Message );
                            oTransaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
    }
}


//var model = new ExcelOperations.ShowroomModel()
//{
//    Name = "Light n Style",
//    Address = "121/1,</span><span>Infantry Road ,;" +
//                    "<span>Bangalore-560001; Ph No: +91 080 41130090/41132190;",
//    BankDetails = "Account Name :- Light and Style; Account Type : Current Account;" +
//                    " Bank Name :- Kotak Mahindra Bank; A/c No. :- 165044005048; IFSC Code :- KKBK0008059; " +
//                    "Branch :- Infantry Road Branch Bangalore;",
//    TermsAndConditions = @"GENERAL TERMS ;1.Price in INR per piece ;
//2.Above mentioned price are including of GST ; 3.Excluding installation. ;4.Validity : Pricing valid for 15 days only. ;
//5.We will not accept cancellation or return of goods, once the order is confirmed;
//;\n;
//PAYMENT TERMS ;
//1. 100 % advance Payment along with PO &order confirmation;
//;\n;
//DELIVERY TERMS ;
//1. 3 - 4 days after receipt of confirm order with advance;
//2.Delivery in Bangalore.Outside transportation charges extra at actuals.;\n;
//We sincerely hope our offer stands in line with the requirement and helps you in your evaluation and decision making.Looking forward to your most valued order ;
//;\n;
//Thanking and assuring you of our best services at all times ",
//    Users = new List<int> { 1 },
//    Catalogues = new List<int> { 1, 2, 3, 4, 5, 6, 7 },
//    Code = "lns@rousing",
//    ShowroomImage = File.ReadAllBytes(@"C:\Users\844617\source\repos\RQuote Quotation Application\RQuote.WPF\Assets\pdficon.jpeg")
//};
//excel.AddShowRoom(model);