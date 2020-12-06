using System.Collections.Generic;

namespace RQuote.Data.Tables
{
    public class Showrooms
    {
        public int Id { get; set; }
        public string  Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Code { get; set; }
        public List<Catalogue> Catalogues { get; set; }
        public List<Users> Users { get; set; }

    }

}
