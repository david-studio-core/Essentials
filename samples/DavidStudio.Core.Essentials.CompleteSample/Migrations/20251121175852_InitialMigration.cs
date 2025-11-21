using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DavidStudio.Core.Essentials.CompleteSample.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IncorporationDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockCount = table.Column<int>(type: "int", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "Id", "IncorporationDateUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("e9a85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(1976, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Apple" },
                    { new Guid("eaa85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(1938, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Samsung" },
                    { new Guid("eba85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2010, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xiaomi" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedByUserId", "IsDeleted", "ManufacturerId", "ModifiedAtUtc", "ModifiedByUserId", "Name", "Price", "StockCount" },
                values: new object[,]
                {
                    { new Guid("eca85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), false, new Guid("e9a85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "iPhone 17", 1199m, 1000 },
                    { new Guid("eda85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), false, new Guid("e9a85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "iPhone 16", 999m, 50 },
                    { new Guid("eea85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), false, new Guid("eaa85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Samsung Galaxy S25", 600m, 25 },
                    { new Guid("efa85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000000-0000-0000-0000-000000000000"), false, new Guid("eba85b6d-b003-46af-ad15-69b1a6cbdd7c"), new DateTime(2025, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Xiaomi 17 Pro Max", 1500m, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
