using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class songsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_Songs",
                columns: table => new
                {
                    SongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlbumID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Songs", x => x.SongID);
                    table.ForeignKey(
                        name: "FK_tbl_Songs_tbl_Albums_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "tbl_Albums",
                        principalColumn: "AlbumID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Songs_AlbumID",
                table: "tbl_Songs",
                column: "AlbumID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_Songs");
        }
    }
}
