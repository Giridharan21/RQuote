namespace RQuote.Data
{
    using RQuote.Data.Tables;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class QuotationAppDb : DbContext
    {
        
        public QuotationAppDb()
            : base("QuotationAppDb")
        {
        }
        
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Showrooms> Showrooms { get; set; }
        public virtual DbSet<Catalogue> Catalogues { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<ProductProperties> ProductProperties { get; set; }
        public virtual DbSet<Quotations> Quotations { get; set; }
        public virtual DbSet<QuotationProduct> QuotationProduct { get; set; }

    }
    
    
}