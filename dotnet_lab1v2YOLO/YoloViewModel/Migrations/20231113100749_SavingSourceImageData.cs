using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoloViewModel.Migrations
{
    /// <inheritdoc />
    public partial class SavingSourceImageData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "SourceImageData",
                table: "Images",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceImageData",
                table: "Images");
        }
    }
}
