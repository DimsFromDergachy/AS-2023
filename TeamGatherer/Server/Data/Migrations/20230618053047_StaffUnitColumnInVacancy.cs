using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamGatherer.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class StaffUnitColumnInVacancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StateUnitId",
                table: "Vacancies",
                newName: "StaffUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StaffUnitId",
                table: "Vacancies",
                newName: "StateUnitId");
        }
    }
}
