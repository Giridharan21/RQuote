namespace RQuote.Data.Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Quotation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quotation()
        {
            QuotationProducts = new HashSet<QuotationProduct>();
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string ClientName { get; set; }

        public string ClientEmail { get; set; }

        public string ClientPhone { get; set; }

        public string ClientOrganization { get; set; }

        public string ProjectName { get; set; }

        public double TotalAmt { get; set; }

        public int CreatedById { get; set; }

        public bool Uploaded { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuotationProduct> QuotationProducts { get; set; }

        public virtual User User { get; set; }
    }
}
