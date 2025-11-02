using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAppTrafficSign.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionLocationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Contributions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Contributions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Contributions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Contributions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Contributions");
        }
    }
}
