using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddJournal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JournalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resolved = table.Column<bool>(type: "bit", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Journals_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BloodTestJournal",
                columns: table => new
                {
                    BloodTestsId = table.Column<int>(type: "int", nullable: false),
                    JournalsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloodTestJournal", x => new { x.BloodTestsId, x.JournalsId });
                    table.ForeignKey(
                        name: "FK_BloodTestJournal_BloodTests_BloodTestsId",
                        column: x => x.BloodTestsId,
                        principalTable: "BloodTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BloodTestJournal_Journals_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatJournal",
                columns: table => new
                {
                    ChatsId = table.Column<int>(type: "int", nullable: false),
                    JournalsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatJournal", x => new { x.ChatsId, x.JournalsId });
                    table.ForeignKey(
                        name: "FK_ChatJournal_Chats_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatJournal_Journals_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalJournal",
                columns: table => new
                {
                    GoalsId = table.Column<int>(type: "int", nullable: false),
                    JournalsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalJournal", x => new { x.GoalsId, x.JournalsId });
                    table.ForeignKey(
                        name: "FK_GoalJournal_Goals_GoalsId",
                        column: x => x.GoalsId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalJournal_Journals_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JournalWorkout",
                columns: table => new
                {
                    JournalsId = table.Column<int>(type: "int", nullable: false),
                    WorkoutsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalWorkout", x => new { x.JournalsId, x.WorkoutsId });
                    table.ForeignKey(
                        name: "FK_JournalWorkout_Journals_JournalsId",
                        column: x => x.JournalsId,
                        principalTable: "Journals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalWorkout_Workouts_WorkoutsId",
                        column: x => x.WorkoutsId,
                        principalTable: "Workouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BloodTestJournal_JournalsId",
                table: "BloodTestJournal",
                column: "JournalsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatJournal_JournalsId",
                table: "ChatJournal",
                column: "JournalsId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalJournal_JournalsId",
                table: "GoalJournal",
                column: "JournalsId");

            migrationBuilder.CreateIndex(
                name: "IX_Journals_PersonId",
                table: "Journals",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalWorkout_WorkoutsId",
                table: "JournalWorkout",
                column: "WorkoutsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BloodTestJournal");

            migrationBuilder.DropTable(
                name: "ChatJournal");

            migrationBuilder.DropTable(
                name: "GoalJournal");

            migrationBuilder.DropTable(
                name: "JournalWorkout");

            migrationBuilder.DropTable(
                name: "Journals");
        }
    }
}
