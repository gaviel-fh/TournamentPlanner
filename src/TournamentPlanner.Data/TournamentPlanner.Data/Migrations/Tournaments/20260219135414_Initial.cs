using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlanner.Data.Migrations.Tournaments
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Country = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TournamentDisciplines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundCount = table.Column<int>(type: "integer", nullable: false),
                    BoutIntervalMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentDisciplines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentDisciplines_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentDisciplines_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentMembers_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentDisciplineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantAUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantBUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledStartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    WinnerUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bouts_TournamentDisciplines_TournamentDisciplineId",
                        column: x => x.TournamentDisciplineId,
                        principalTable: "TournamentDisciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoutRounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    ParticipantAScore = table.Column<int>(type: "integer", nullable: false),
                    ParticipantBScore = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoutRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoutRounds_Bouts_BoutId",
                        column: x => x.BoutId,
                        principalTable: "Bouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoutRoundId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardedToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwardedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoreEvents_BoutRounds_BoutRoundId",
                        column: x => x.BoutRoundId,
                        principalTable: "BoutRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoutRounds_BoutId_RoundNumber",
                table: "BoutRounds",
                columns: new[] { "BoutId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bouts_TournamentDisciplineId_ScheduledStartUtc",
                table: "Bouts",
                columns: new[] { "TournamentDisciplineId", "ScheduledStartUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_Code",
                table: "Disciplines",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScoreEvents_BoutRoundId_OccurredAtUtc",
                table: "ScoreEvents",
                columns: new[] { "BoutRoundId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDisciplines_DisciplineId",
                table: "TournamentDisciplines",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentDisciplines_TournamentId_DisciplineId",
                table: "TournamentDisciplines",
                columns: new[] { "TournamentId", "DisciplineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TournamentMembers_TournamentId_UserId_Role",
                table: "TournamentMembers",
                columns: new[] { "TournamentId", "UserId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_StartDateUtc_EndDateUtc",
                table: "Tournaments",
                columns: new[] { "StartDateUtc", "EndDateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_VenueId",
                table: "Tournaments",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScoreEvents");

            migrationBuilder.DropTable(
                name: "TournamentMembers");

            migrationBuilder.DropTable(
                name: "BoutRounds");

            migrationBuilder.DropTable(
                name: "Bouts");

            migrationBuilder.DropTable(
                name: "TournamentDisciplines");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
