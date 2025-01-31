using E_Ticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Localhost
            //optionsBuilder.UseSqlServer(@"Server=LAPTOP-EMJSQFU7\SQLEXPRESS;Database=EticaretDb;Trusted_Connection=true;TrustServerCertificate=True; ");
            // base.OnConfiguring(optionsBuilder);


            // Free Host
           //optionsBuilder.UseSqlServer(@"workstation id=Eticaret20.mssql.somee.com;packet size=4096;user id=KayaAhmet_SQLLogin_2;pwd=x2mm32q11q;data source=Eticaret20.mssql.somee.com;persist security info=False;initial catalog=Eticaret20;TrustServerCertificate=True");
            // base.OnConfiguring(optionsBuilder);

           // optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            // modelBuilder.ApplyConfiguration(new BrandConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // ÇALIŞAN DLL'iN İÇİNDEN BUL OTOMATİK OLARAK YAPAR

            base.OnModelCreating(modelBuilder);
        }
    }
}
