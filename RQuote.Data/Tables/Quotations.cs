using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RQuote.Data.Tables
{
    public class Quotations
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }
        public string ClientOrganization { get; set; }
        public string ProjectName { get; set; }
        public double TotalAmt { get; set; }
        public Users CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]

        public int CreatedById { get; set; }
        [DefaultValue(false)]
        public bool Uploaded { get; set; }
        public List<QuotationProduct> QuotationProducts { get; set; }

    }

}
