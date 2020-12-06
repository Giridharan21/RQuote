using RQuote.Data.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQuote.Data.Server
{
    public static class RQuoteServer
    {
        public static void GetData()
        {
            SqlConnection con = new SqlConnection(@"Data Source=SQL5086.site4now.net;Initial Catalog=DB_A6B5D4_RQuoteServer2;User Id=DB_A6B5D4_RQuoteServer2_admin;Password=RQuote123!;");
            SqlDataAdapter adapter = new SqlDataAdapter("select * from Catalogues", con);
            DataSet dt = new DataSet();
            adapter.Fill(dt);
            var table = dt.Tables[0];
            foreach (DataRow x in table.Rows)
            {
                foreach (var y in x.ItemArray)
                {
                    Console.Write(y +" ");
                }
                Console.WriteLine();
            }
        }
    }
}
