using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWinningOutcomeToMarket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "WinningOutcomeIndex",
                table: "Markets",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinningOutcomeIndex",
                table: "Markets");
        }
    }
}
