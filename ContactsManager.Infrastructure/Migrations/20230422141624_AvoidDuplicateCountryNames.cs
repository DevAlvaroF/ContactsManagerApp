using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
	public partial class AvoidDuplicateCountryNames : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "CountryName",
				table: "Countries",
				type: "nvarchar(450)",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Countries_CountryName",
				table: "Countries",
				column: "CountryName",
				unique: true,
				filter: "[CountryName] IS NOT NULL");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_Countries_CountryName",
				table: "Countries");

			migrationBuilder.AlterColumn<string>(
				name: "CountryName",
				table: "Countries",
				type: "nvarchar(max)",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "nvarchar(450)",
				oldNullable: true);
		}
	}
}
