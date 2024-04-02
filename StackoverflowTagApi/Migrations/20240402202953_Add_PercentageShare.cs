using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackoverflowTagApi.Migrations
{
    /// <inheritdoc />
    public partial class Add_PercentageShare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PercentageShare",
                table: "Tags",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentageShare",
                table: "Tags");
        }
    }
}
