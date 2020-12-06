using System;
using System.Collections.Generic;

namespace RQuote.Data.Tables
{
    public class Catalogue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Showrooms> Showrooms { get; set; }
        public DateTime DateCreated { get; set; }

    }

}
