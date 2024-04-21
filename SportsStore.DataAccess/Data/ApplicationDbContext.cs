using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, DisplayOrder = 1, Name = "Одежда", ImageURL = "" },
                new Category { Id = 2, DisplayOrder = 2, Name = "Обувь", ImageURL = "" },
                new Category { Id = 3, DisplayOrder = 3, Name = "Тренажёры", ImageURL = "" },
                new Category { Id = 4, DisplayOrder = 4, Name = "Аксессуары", ImageURL = "" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Футболка мужская",
                    Description = "Ткань: хлопок, размер: XL, цвет: синий",
                    Price = 800,
                    CategoryId = 2026
                },
                new Product
                {
                    Id = 2,
                    Name = "Лыжи",
                    Description = "геометрия: 120/73/103 мм, ростовка: 165см, радиус выреза: R12",
                    Price = 15000,
                    CategoryId = 2028
                },
                new Product
                {
                    Id = 3,
                    Name = "Шапка для плавания",
                    Description = "Материал: резина",
                    Price = 500,
                    CategoryId = 2025
                },
                new Product
                {
                    Id = 4,
                    Name = "Гантели",
                    Description = "Вес: 5 кг, материал: сталь",
                    Price = 2000,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 5,
                    Name = "Беговая дорожка",
                    Description = "Вес: 53 кг, габариты: 155x73x31 см, Ширина полотна: 45 см",
                    Price = 50000,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 6,
                    Name = "Мяч баскетбольный",
                    Description = "Материал: резина, цвет: оранжевый",
                    Price = 750,
                    CategoryId = 1022
                }
                );
        }
    }
}
