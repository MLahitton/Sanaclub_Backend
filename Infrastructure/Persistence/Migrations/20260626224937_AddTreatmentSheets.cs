using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentSheets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "treatment_sheets",
                schema: "clinical",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    treatment_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    treatment_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consultation_date = table.Column<DateOnly>(type: "date", nullable: true),
                    eps_treating_doctor_diagnosis = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    referred_clinical_history = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    indication_date = table.Column<DateOnly>(type: "date", nullable: true),
                    entry_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    exit_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    assigned_staff_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    therapy_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    nervous_system_indications = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    decompress_spine = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    decompress_neck = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    decompress_back = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    endocrine_nerves = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    endocrine_defenses = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    endocrine_hormones = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    cardiovascular_reflexology_with = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    digestive_colon_reflexology_with = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    respiratory_reflexology_with = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    urinary_reflexology_with_acid_fruits = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    other_indications = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    observations = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatment_sheets", x => x.id);
                    table.ForeignKey(
                        name: "FK_treatment_sheets_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_treatment_sheets_treatment_statuses_treatment_status_id",
                        column: x => x.treatment_status_id,
                        principalSchema: "catalog",
                        principalTable: "treatment_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_treatment_sheets_created_at_utc",
                schema: "clinical",
                table: "treatment_sheets",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_sheets_patient_id",
                schema: "clinical",
                table: "treatment_sheets",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatment_sheets_treatment_status_id",
                schema: "clinical",
                table: "treatment_sheets",
                column: "treatment_status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "treatment_sheets",
                schema: "clinical");
        }
    }
}
