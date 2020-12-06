using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQuote
{
    public static class CustomProductManager
    {
        public static List<Product> LoadCustomCatalogue()
        {
            List<Product> products = new List<Product>();
            String customProductFilePath = Utils.CustomProductsFilePath;
            if (File.Exists(customProductFilePath))
            {
                string fileContent = Utils.Decrypt(File.ReadAllText(customProductFilePath));
                var customProds = JsonConvert.DeserializeObject<List<Product>>(fileContent);
                if (customProds != null)
                {
                    products.AddRange(customProds);
                }
            }
            return products;
        }

        public static void SaveCustomProduct(Product newProduct)
        {
            List<Product> products = new List<Product>();
            String customProductFilePath = Utils.CustomProductsFilePath;
            if (File.Exists(customProductFilePath))
            {
                products.AddRange(JsonConvert.DeserializeObject<List<Product>>(Utils.Decrypt(File.ReadAllText(customProductFilePath))));
            }
            else
            {
                File.WriteAllText(customProductFilePath, Utils.Encrypt("[]"));
            }

            products.Add(newProduct);

            if (products.Count > 0)
            {
                File.WriteAllText(customProductFilePath, Utils.Encrypt(JsonConvert.SerializeObject(products)));
            }
        }
    }
}
