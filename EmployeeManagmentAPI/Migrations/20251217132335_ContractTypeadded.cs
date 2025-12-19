using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagmentAPI.Migrations
{
    /// <inheritdoc />
    public partial class ContractTypeadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContractType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractType",
                table: "AspNetUsers");
        }
    }
}
