using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.DataAccess.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, DisplayOrder = 1, Name = "Одежда"},
                new Category { Id = 2, DisplayOrder = 2, Name = "Обувь"},
                new Category { Id = 3, DisplayOrder = 3, Name = "Тренажёры"},
                new Category { Id = 4, DisplayOrder = 4, Name = "Аксессуары"}
            );
        }
    }
}
