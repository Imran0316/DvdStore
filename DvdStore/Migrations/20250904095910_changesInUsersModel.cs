using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DvdStore.Migrations
{
    /// <inheritdoc />
    public partial class changesInUsersModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "tbl_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "tbl_Users");
        }
    }
}
