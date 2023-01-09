using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinanceWebSocketTask.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SymbolAveragePrices",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    AveragePrice = table.Column<double>(type: "REAL", nullable: false),
                    AveragePriceTimeSpanInMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    PreviousAveragePrices = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymbolAveragePrices", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "SymbolSimpleMovingAveragePriceDataPoints",
                columns: table => new
                {
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    TimeInterval = table.Column<string>(type: "TEXT", nullable: false),
                    AveragePrice = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymbolSimpleMovingAveragePriceDataPoints", x => x.CreatedOn);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SymbolSimpleMovingAveragePriceDataPoints_TimeInterval",
                table: "SymbolSimpleMovingAveragePriceDataPoints",
                column: "TimeInterval");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SymbolAveragePrices");

            migrationBuilder.DropTable(
                name: "SymbolSimpleMovingAveragePriceDataPoints");
        }
    }
}
