using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQuote.Logic.Models
{
    public class SavedQuotationModel
    {
        public SavedQuotationModel(string quotationDate, string quotationNumber, string quotationCustomer)
        {
            QuotationDate = quotationDate;
            QuotationNumber = quotationNumber;
            QuotationCustomer = quotationCustomer;
        }

        public string QuotationDate { get; set; }
        public string QuotationNumber { get; set; }
        public string QuotationCustomer { get; set; }
        public string Id { get; set; }
    }
}
