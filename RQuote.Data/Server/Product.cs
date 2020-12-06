namespace RQuote.Data.Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            ProductProperties = new HashSet<ProductProperty>();
            QuotationProducts = new HashSet<QuotationProduct>();
        }

        public int Id { get; set; }

        public string Details { get; set; }

        public double Price { get; set; }

        public int CatalogueId { get; set; }

        public string ModelNo { get; set; }

        public byte[] Image { get; set; }

        public bool Downloaded { get; set; }

        public virtual Catalogue Catalogue { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductProperty> ProductProperties { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationProduct> QuotationProducts { get; set; }
    }
}
