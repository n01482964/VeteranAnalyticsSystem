using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeteranAnalyticsSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "Veterans",
                columns: table => new
                {
                    VeteranId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HomeAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelationshipStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MilitaryServiceStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HighestRank = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StartOfService = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndOfService = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchOfService = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NumberOfDeployments = table.Column<int>(type: "int", nullable: false),
                    DeploymentDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HealthConcerns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalHealthInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhysicalLimitations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetreatDateLocation = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterans", x => x.VeteranId);
                });

            migrationBuilder.CreateTable(
                name: "Volunteers",
                columns: table => new
                {
                    VolunteerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteers", x => x.VolunteerId);
                });

            migrationBuilder.CreateTable(
                name: "EventVeteran",
                columns: table => new
                {
                    EventsEventId = table.Column<int>(type: "int", nullable: false),
                    ParticipantsVeteranId = table.Column<string>(type: "nvarchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventVeteran", x => new { x.EventsEventId, x.ParticipantsVeteranId });
                    table.ForeignKey(
                        name: "FK_EventVeteran_Events_EventsEventId",
                        column: x => x.EventsEventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventVeteran_Veterans_ParticipantsVeteranId",
                        column: x => x.ParticipantsVeteranId,
                        principalTable: "Veterans",
                        principalColumn: "VeteranId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeteranId = table.Column<string>(type: "nvarchar(64)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Responses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_Surveys_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Surveys_Veterans_VeteranId",
                        column: x => x.VeteranId,
                        principalTable: "Veterans",
                        principalColumn: "VeteranId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventVeteran_ParticipantsVeteranId",
                table: "EventVeteran",
                column: "ParticipantsVeteranId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_EventId",
                table: "Surveys",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_VeteranId",
                table: "Surveys",
                column: "VeteranId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventVeteran");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "Volunteers");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Veterans");
        }
    }
}
