using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEnviMonitoring.API.Migrations
{
    /// <inheritdoc />
    public partial class device_refactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Devices");
        }
    }
}
