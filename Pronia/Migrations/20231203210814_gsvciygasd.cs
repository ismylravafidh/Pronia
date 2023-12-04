using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pronia.Migrations
{
    /// <inheritdoc />
    public partial class gsvciygasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "ProductImages",
                newName: "AdditionImgUrl");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrime",
                table: "ProductImages",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "HoverImgUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImgUrl",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoverImgUrl",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "MainImgUrl",
                table: "ProductImages");

            migrationBuilder.RenameColumn(
                name: "AdditionImgUrl",
                table: "ProductImages",
                newName: "ImgUrl");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrime",
                table: "ProductImages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
