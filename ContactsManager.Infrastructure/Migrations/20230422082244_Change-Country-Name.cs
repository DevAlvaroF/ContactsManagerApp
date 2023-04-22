using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    public partial class ChangeCountryName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "CountryName",
                value: "Mexico");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                column: "CountryName",
                value: "China");
        }
    }
}
