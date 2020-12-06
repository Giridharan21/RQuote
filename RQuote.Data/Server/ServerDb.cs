namespace RQuote.Data.Server
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ServerDb : DbContext
    {
        public ServerDb()
            : base("ServerDbContext")
        {
        }

        public virtual DbSet<Catalogue> Catalogues { get; set; }
        public virtual DbSet<ProductProperty> ProductProperties { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<QuotationProduct> QuotationProducts { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<Showroom> Showrooms { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Catalogue>()
                .HasMany(e => e.Showrooms)
                .WithMany(e => e.Catalogues)
                .Map(m => m.ToTable("ShowroomsCatalogues").MapRightKey("Showrooms_Id"));

            modelBuilder.Entity<Quotation>()
                .HasMany(e => e.QuotationProducts)
                .WithOptional(e => e.Quotation)
                .HasForeignKey(e => e.Quotations_Id);

            modelBuilder.Entity<Showroom>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Showrooms)
                .Map(m => m.ToTable("UsersShowrooms").MapLeftKey("Showrooms_Id").MapRightKey("Users_Id"));

            modelBuilder.Entity<User>()
                .HasMany(e => e.Quotations)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.CreatedById);
        }
    }
}
