using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Downloads",
                columns: table => new
                {
                    DownloadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SongID = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    DownloadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Downloads", x => x.DownloadID);
                    table.ForeignKey(
                        name: "FK_tbl_Downloads_tbl_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "tbl_Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK_tbl_Downloads_tbl_Songs_SongID",
                        column: x => x.SongID,
                        principalTable: "tbl_Songs",
                        principalColumn: "SongID");
                    table.ForeignKey(
                        name: "FK_tbl_Downloads_tbl_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "tbl_Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Downloads_ProductID",
                table: "tbl_Downloads",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Downloads_SongID",
                table: "tbl_Downloads",
                column: "SongID");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Downloads_UserID",
                table: "tbl_Downloads",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Downloads");
        }
    }
}
