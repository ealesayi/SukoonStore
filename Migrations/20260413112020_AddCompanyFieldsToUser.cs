using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebProject.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyLogoUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyLogoUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");
        }
    }
}
