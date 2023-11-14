using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoloViewModel.Migrations
{
    /// <inheritdoc />
    public partial class RemovingTuples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sizesH",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sizesW",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sourceSizesH",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sourceSizesW",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sizesH",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "sizesW",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "sourceSizesH",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "sourceSizesW",
                table: "Images");
        }
    }
}
