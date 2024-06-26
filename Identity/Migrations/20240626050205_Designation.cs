using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Migrations
{
    /// <inheritdoc />
    public partial class Designation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Designation",
                table: "AspNetUsers",
                newName: "WorkExperience");

            migrationBuilder.AddColumn<string>(
                name: "EmpDesignation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeLocation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpDesignation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OfficeLocation",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WorkExperience",
                table: "AspNetUsers",
                newName: "Designation");
        }
    }
}
