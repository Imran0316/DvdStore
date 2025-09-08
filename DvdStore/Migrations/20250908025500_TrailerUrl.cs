using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class TrailerUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Albums_tbl_Artists_ArtistID",
                table: "tbl_Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Albums_tbl_Category_CategoryID",
                table: "tbl_Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
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

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "tbl_Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "tbl_Albums",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tbl_Albums",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "tbl_Albums",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ArtistID",
                table: "tbl_Albums",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Albums_tbl_Artists_ArtistID",
                table: "tbl_Albums",
                column: "ArtistID",
                principalTable: "tbl_Artists",
                principalColumn: "ArtistID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Albums_tbl_Category_CategoryID",
                table: "tbl_Albums",
                column: "CategoryID",
                principalTable: "tbl_Category",
                principalColumn: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products",
                column: "AlbumID",
                principalTable: "tbl_Albums",
                principalColumn: "AlbumID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Albums_tbl_Artists_ArtistID",
                table: "tbl_Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Albums_tbl_Category_CategoryID",
                table: "tbl_Albums");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products");

            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "tbl_Products");

            migrationBuilder.AlterColumn<int>(
                name: "AlbumID",
                table: "tbl_Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "tbl_Albums",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "tbl_Albums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryID",
                table: "tbl_Albums",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArtistID",
                table: "tbl_Albums",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Albums_tbl_Artists_ArtistID",
                table: "tbl_Albums",
                column: "ArtistID",
                principalTable: "tbl_Artists",
                principalColumn: "ArtistID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Albums_tbl_Category_CategoryID",
                table: "tbl_Albums",
                column: "CategoryID",
                principalTable: "tbl_Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_Products_tbl_Albums_AlbumID",
                table: "tbl_Products",
                column: "AlbumID",
                principalTable: "tbl_Albums",
                principalColumn: "AlbumID");
        }
    }
}
