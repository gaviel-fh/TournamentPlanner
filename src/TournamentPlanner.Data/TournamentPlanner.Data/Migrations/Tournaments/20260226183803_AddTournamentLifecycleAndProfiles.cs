using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlanner.Data.Migrations.Tournaments
{
    /// <inheritdoc />
    public partial class AddTournamentLifecycleAndProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Venues",
                type: "double precision",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Venues",
                type: "double precision",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignupEndDateUtc",
                table: "Tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SignupStartDateUtc",
                table: "Tournaments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ParticipantATotalScore",
                table: "Bouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantBTotalScore",
                table: "Bouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TournamentUserProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentUserProfiles", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_SignupEndDateUtc",
                table: "Tournaments",
                column: "SignupEndDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Bouts_WinnerUserId_Status",
                table: "Bouts",
                columns: new[] { "WinnerUserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TournamentUserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_SignupEndDateUtc",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Bouts_WinnerUserId_Status",
                table: "Bouts");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "SignupEndDateUtc",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "SignupStartDateUtc",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "ParticipantATotalScore",
                table: "Bouts");

            migrationBuilder.DropColumn(
                name: "ParticipantBTotalScore",
                table: "Bouts");
        }
    }
}
