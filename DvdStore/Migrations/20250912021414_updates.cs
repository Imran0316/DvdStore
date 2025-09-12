using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_NewsPromotions_tbl_Products_ProductID",
                table: "tbl_NewsPromotions");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_NewsPromotions_tbl_Products_ProductID",
                table: "tbl_NewsPromotions",
                column: "ProductID",
                principalTable: "tbl_Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_NewsPromotions_tbl_Products_ProductID",
                table: "tbl_NewsPromotions");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_NewsPromotions_tbl_Products_ProductID",
                table: "tbl_NewsPromotions",
                column: "ProductID",
                principalTable: "tbl_Products",
                principalColumn: "ProductID");
        }
    }
}
