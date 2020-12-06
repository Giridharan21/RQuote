using RQuote.Data;
using RQuote.Data.Server;
using RQuote.Data.Tables;
using RQuote.Logic;

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
        static void Main(string[] args)
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
            InsertProducts(products);

            return;

            //foreach (var item in headers)
            //{
            //    //Console.WriteLine(item);
            //}
            Console.WriteLine("Finished");
            int i = 0;
            var cmds = new List<string>();
            var details = excel.GetProductDetails();
            foreach (var item in details)
            {
                i++;

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
                var str = $"Insert into ProductProperties(ProductId,[{string.Join("],[", cols)}]) values({i},'{string.Join("','", val)}');";
                cmds.Add(str);
                //Console.WriteLine(str);
                //Console.ReadLine();
                //context.Database.ExecuteSqlCommand
            }
            File.WriteAllLines(@"C:\Users\844617\Downloads\Queries.txt", cmds);
            //context.SaveChanges();
            Console.WriteLine($"finished");
            Console.WriteLine("Finished");
            Console.ReadLine();
            //foreach (var x in excel.GetHeaders())
            //{
            //    count++;
            //    var y = "public string " + string.Join("",x.Where(i=>i!=' '&&i!='/'&&i!='-'&&i!='.'))+ " { get; set; }";
            //    //Console.WriteLine(y);
            //    output += string.Join("", x.Where(i => i != ' ' && i != '/' && i != '-' && i != '.')) + "\t";
            //}
            //rows.Add(output);
            ////Console.WriteLine(count);
            //foreach (var x in excel.GetRowsValues().Take(10))
            //{
            //    Console.WriteLine(x);
            //    Console.WriteLine();
            //    //rows.Add(string.Join("\t", x));
            //}
            //File.WriteAllLines(@"C:\Users\844617\Downloads\check.txt",rows);


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
