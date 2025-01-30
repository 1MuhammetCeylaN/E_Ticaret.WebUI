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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x=>x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Image).IsRequired().HasMaxLength(155);
            builder.HasData(
                new Category
                {
                    Name="Elektronik",
                    Id = 1,
                    IsActive = true,
                    IsTopMenu = true,
                    ParentId = 0,
                    OrderNo = 1,
                    Image = "1.jpg"
                },
                new Category
                {
                    Name = "Bilgisayar",
                    Id = 2,
                    IsActive = true,
                    IsTopMenu = true,
                    ParentId = 0,
                    OrderNo = 2,
                    Image = "2.jpg"
                }
            );
        }
    }
}
