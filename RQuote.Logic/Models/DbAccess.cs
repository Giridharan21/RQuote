
using RQuote.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RQuote.Logic.Models
{
    public class DbAccess
    {
        private ServerDbContext DbContext;
        public DbAccess()
        {
            DbContext = new ServerDbContext();
        }

        public async Task<List<string>> GetUsersForShowRoom(string ShowroomCode)
        {
            
            var users =await Task.Run(()=> DbContext.Showrooms.Include("Users").Where(i => i.Code == ShowroomCode).
                Select(i=>i.Users).FirstOrDefault());
            
            if (users is null || users.Count()==0)
                return new List<string>();
            return users.Select(i => i.Name).ToList();
            
        }

        public IEnumerable GetModelNumbers(string filter)
        {

            filter = filter.ToLower();
            var values = DbContext.Products.Where(i => i.ModelNo.ToLower().Contains(filter));
            return values;
                
        }

        
        public async Task<bool> AuthenticateUser(string username,string password)
        {
            return await Task.Run(()=> DbContext.Users.Where(i => i.Name == username && i.Password == password).FirstOrDefault() != null);
        }

        public async Task<List<SavedQuotationModel>> GetQuotations()
        {
            var list = new List<SavedQuotationModel>();
            await Task.Run(() =>
            {
                var x = DbContext.Quotations.Select(i => new { i.Date, i.ClientOrganization, i.Id });
                foreach (var i in x)
                {
                    list.Add(new SavedQuotationModel(i.Date.ToShortDateString(), i.Id.ToString(), i.ClientOrganization) { Id = i.Id.ToString() }) ;
                }
            });
            return list;
        }
       
    }
}
