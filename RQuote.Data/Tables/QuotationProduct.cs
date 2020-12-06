using System.ComponentModel.DataAnnotations.Schema;

namespace RQuote.Data.Tables
{
    public class QuotationProduct
    {
        public int Id { get; set; }
        public Products Product { get; set; }
        
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Unit { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public double GST { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        
    }

}
