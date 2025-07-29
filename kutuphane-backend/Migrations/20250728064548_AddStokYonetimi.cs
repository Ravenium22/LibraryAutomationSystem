using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kutuphane.Migrations
{
    /// <inheritdoc />
    public partial class AddStokYonetimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kitap_MusaitMi",
                table: "Kitaplar");

            migrationBuilder.DropColumn(
                name: "MusaitMi",
                table: "Kitaplar");

            migrationBuilder.AddColumn<int>(
                name: "MusaitStok",
                table: "Kitaplar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToplamStok",
                table: "Kitaplar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusaitStok",
                table: "Kitaplar");

            migrationBuilder.DropColumn(
                name: "ToplamStok",
                table: "Kitaplar");

            migrationBuilder.AddColumn<bool>(
                name: "MusaitMi",
                table: "Kitaplar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Kitap_MusaitMi",
                table: "Kitaplar",
                column: "MusaitMi");
        }
    }
}
