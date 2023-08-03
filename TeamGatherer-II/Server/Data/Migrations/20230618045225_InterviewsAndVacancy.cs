using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TeamGatherer.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class InterviewsAndVacancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StateUnitId",
                table: "Vacancies",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InterviewResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpertId = table.Column<string>(type: "text", nullable: true),
                    InterviewId = table.Column<int>(type: "integer", nullable: false),
                    Estimations = table.Column<List<SkillEstimation>>(type: "jsonb", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VacancyId = table.Column<int>(type: "integer", nullable: false),
                    StartTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    EndTimestamp = table.Column<long>(type: "bigint", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Candidate = table.Column<Candidate>(type: "jsonb", nullable: true),
                    HrIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    ExpertIds = table.Column<List<string>>(type: "text[]", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewResults");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropColumn(
                name: "StateUnitId",
                table: "Vacancies");
        }
    }
}
