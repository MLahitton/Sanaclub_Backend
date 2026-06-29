using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentsMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointment_statuses",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                schema: "clinical",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    treatment_sheet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    clinical_reference_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    clinical_reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    therapist_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    appointment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    appointment_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_name_snapshot = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: true),
                    scheduled_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    confirmed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    confirmed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointments_appointment_statuses_appointment_status_id",
                        column: x => x.appointment_status_id,
                        principalSchema: "catalog",
                        principalTable: "appointment_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_treatment_sheets_treatment_sheet_id",
                        column: x => x.treatment_sheet_id,
                        principalSchema: "clinical",
                        principalTable: "treatment_sheets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_therapist_user_id",
                        column: x => x.therapist_user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_appointment_statuses_code",
                schema: "catalog",
                table: "appointment_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_appointment_statuses_is_active",
                schema: "catalog",
                table: "appointment_statuses",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_statuses_sort_order",
                schema: "catalog",
                table: "appointment_statuses",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_appointment_status_id",
                schema: "clinical",
                table: "appointments",
                column: "appointment_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_clinical_reference_type_clinical_reference_id",
                schema: "clinical",
                table: "appointments",
                columns: new[] { "clinical_reference_type", "clinical_reference_id" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_is_active",
                schema: "clinical",
                table: "appointments",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_patient_id",
                schema: "clinical",
                table: "appointments",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_therapist_user_id_appointment_date",
                schema: "clinical",
                table: "appointments",
                columns: new[] { "therapist_user_id", "appointment_date" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_therapist_user_id_appointment_date_start_time_~",
                schema: "clinical",
                table: "appointments",
                columns: new[] { "therapist_user_id", "appointment_date", "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_treatment_sheet_id",
                schema: "clinical",
                table: "appointments",
                column: "treatment_sheet_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments",
                schema: "clinical");

            migrationBuilder.DropTable(
                name: "appointment_statuses",
                schema: "catalog");
        }
    }
}
