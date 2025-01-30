using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Ticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Ticaret.Data.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.SurName).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Phone).IsRequired().HasColumnType("varchar(15)").HasMaxLength(15);
            builder.Property(x => x.Password).IsRequired().HasColumnType("nvarchar(25)").HasMaxLength(25);
            builder.Property(x => x.UserName).HasColumnType("nvarchar(25)").HasMaxLength(25);
            builder.HasData(new AppUser
            {
                Id = 1,
                UserName = "Admin",
                Email = "admin@gmail.com",
                Phone = "12345",
                IsActive = true,
                IsAdmin = true,
                Name = "adminname",
                SurName = "adminsurname",
                Password = "123456*",
            });
        }
    }
}
