using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class ProductReviewAndNewsPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_NewsPromotions",
                columns: table => new
                {
                    NewsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_NewsPromotions", x => x.NewsID);
                    table.ForeignKey(
                        name: "FK_tbl_NewsPromotions_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID");
                });

            migrationBuilder.CreateTable(
                name: "tbl_ProductReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_ProductReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_tbl_ProductReviews_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_ProductReviews_tbl_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_NewsPromotions_ProductID",
                table: "tbl_NewsPromotions",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ProductReviews_ProductID",
                table: "tbl_ProductReviews",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ProductReviews_UserID",
                table: "tbl_ProductReviews",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_NewsPromotions");

            migrationBuilder.DropTable(
                name: "tbl_ProductReviews");
        }
    }
}
