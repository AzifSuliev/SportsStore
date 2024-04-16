using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAndSeedProductsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Ткань: хлопок, размер: XL, цвет: синий", "Футболка мужская", 800m },
                    { 2, "геометрия: 120/73/103 мм, ростовка: 165см, радиус выреза: R12", "Лыжи", 15000m },
                    { 3, "Материал: резина", "Шапка для плавания", 500m },
                    { 4, "Вес: 5 кг, материал: сталь", "Гантели", 2000m },
                    { 5, "Вес: 53 кг, габариты: 155x73x31 см, Ширина полотна: 45 см", "Беговая дорожка", 50000m },
                    { 6, "Материал: резина, цвет: оранжевый", "Мяч баскетбольный", 750m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
