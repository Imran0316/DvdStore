using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class ProductUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumID",
                table: "tbl_Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ProductImageUrl",
                table: "tbl_Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverImageUrl",
                table: "tbl_Albums",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products",
                column: "AlbumID",
                principalTable: "tbl_Albums",
                principalColumn: "AlbumID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products");

            migrationBuilder.DropColumn(
                name: "ProductImageUrl",
                table: "tbl_Products");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumID",
                table: "tbl_Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverImageUrl",
                table: "tbl_Albums",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products",
                column: "AlbumID",
                principalTable: "tbl_Albums",
                principalColumn: "AlbumID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
