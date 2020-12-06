namespace RQuote.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class QuotationProduct
    {
        public int Id { get; set; }

        public int? Quotations_Id { get; set; }

        public int ProductId { get; set; }

        public int Unit { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double Amount { get; set; }

        public double GST { get; set; }

        public double Discount { get; set; }

        public double TotalAmount { get; set; }

        public virtual Product Product { get; set; }

        public virtual Quotation Quotation { get; set; }
    }
}
