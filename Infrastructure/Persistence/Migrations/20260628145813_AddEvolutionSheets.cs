using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEvolutionSheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "evolution_sheets",
                schema: "clinical",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    treatment_sheet_id = table.Column<Guid>(type: "uuid", nullable: true),
                    evolution_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    therapy_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    evolution_date = table.Column<DateOnly>(type: "date", nullable: true),
                    entry_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    exit_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    assigned_staff_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    therapy_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    evolution_notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    new_indications = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evolution_sheets", x => x.id);
                    table.ForeignKey(
                        name: "FK_evolution_sheets_evolution_statuses_evolution_status_id",
                        column: x => x.evolution_status_id,
                        principalSchema: "catalog",
                        principalTable: "evolution_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evolution_sheets_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_evolution_sheets_treatment_sheets_treatment_sheet_id",
                        column: x => x.treatment_sheet_id,
                        principalSchema: "clinical",
                        principalTable: "treatment_sheets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_evolution_sheets_created_at_utc",
                schema: "clinical",
                table: "evolution_sheets",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_evolution_sheets_evolution_status_id",
                schema: "clinical",
                table: "evolution_sheets",
                column: "evolution_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_evolution_sheets_patient_id",
                schema: "clinical",
                table: "evolution_sheets",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_evolution_sheets_treatment_sheet_id",
                schema: "clinical",
                table: "evolution_sheets",
                column: "treatment_sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "evolution_sheets",
                schema: "clinical");
        }
    }
}
