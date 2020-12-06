using AutoCompleteTextBox.Editors;
using RQuote.Logic.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RQuote.Providers
{
    public sealed class PartSuggestionProvider : ISuggestionProvider
    {
        internal List<Product> ListOfParts { get; set; }
        public DbAccess dbAccess { get; set; }
        //internal List<Product> ListOfParts;
        public IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            //return
            //    ListOfParts
            //        .Where(part => part.ModelNo.ToLower().Contains(filter.ToLower()))
            //        .ToList();

            return dbAccess.GetModelNumbers(filter);
        }

        public PartSuggestionProvider()
        {
            ListOfParts = new List<Product>();
            dbAccess = new DbAccess();
        }
    }
}
