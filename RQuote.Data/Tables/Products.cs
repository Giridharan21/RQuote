using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RQuote.Data.Tables
{
    public class Products
    {
        public int Id { get; set; }
        public string ModelNo { get; set; }
        public string Details { get; set; }
        public double Price { get; set; }
        public byte[] Image { get; set; }
        [DefaultValue(false)]
        public bool Downloaded { get; set; }
        public Catalogue Catalogue { get; set; }
        [ForeignKey("Catalogue")]
        public int CatalogueId { get; set; }

    }

}
