using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AuduoiIP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAdress",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAdress",
                table: "AuditLogs");
        }
    }
}
