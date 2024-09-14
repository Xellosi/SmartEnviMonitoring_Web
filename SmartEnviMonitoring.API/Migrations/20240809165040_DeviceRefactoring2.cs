using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartEnviMonitoring.API.Migrations
{
    /// <inheritdoc />
    public partial class DeviceRefactoring2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceId",
                table: "Devices",
                newName: "DeviceUID");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceUID",
                table: "Devices",
                column: "DeviceUID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceUID",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "DeviceUID",
                table: "Devices",
                newName: "DeviceId");
        }
    }
}
