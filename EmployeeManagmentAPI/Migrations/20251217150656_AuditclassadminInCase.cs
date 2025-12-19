using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class AuditclassadminInCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminInCase",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminInCase",
                table: "AuditLogs");
        }
    }
}
