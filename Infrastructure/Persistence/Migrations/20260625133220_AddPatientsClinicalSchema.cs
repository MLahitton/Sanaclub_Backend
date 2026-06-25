using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientsClinicalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "clinical");

            migrationBuilder.CreateTable(
                name: "patients",
                schema: "clinical",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    identification_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identification_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    gender_id = table.Column<Guid>(type: "uuid", nullable: true),
                    civil_status_id = table.Column<Guid>(type: "uuid", nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    patient_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                    table.ForeignKey(
                        name: "FK_patients_civil_statuses_civil_status_id",
                        column: x => x.civil_status_id,
                        principalSchema: "catalog",
                        principalTable: "civil_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patients_genders_gender_id",
                        column: x => x.gender_id,
                        principalSchema: "catalog",
                        principalTable: "genders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patients_identification_types_identification_type_id",
                        column: x => x.identification_type_id,
                        principalSchema: "catalog",
                        principalTable: "identification_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_patients_patient_statuses_patient_status_id",
                        column: x => x.patient_status_id,
                        principalSchema: "catalog",
                        principalTable: "patient_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_patients_civil_status_id",
                schema: "clinical",
                table: "patients",
                column: "civil_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_gender_id",
                schema: "clinical",
                table: "patients",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "IX_patients_identification_number",
                schema: "clinical",
                table: "patients",
                column: "identification_number");

            migrationBuilder.CreateIndex(
                name: "IX_patients_identification_type_id_identification_number",
                schema: "clinical",
                table: "patients",
                columns: new[] { "identification_type_id", "identification_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patients_is_active",
                schema: "clinical",
                table: "patients",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_patients_last_name",
                schema: "clinical",
                table: "patients",
                column: "last_name");

            migrationBuilder.CreateIndex(
                name: "IX_patients_patient_status_id",
                schema: "clinical",
                table: "patients",
                column: "patient_status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients",
                schema: "clinical");
        }
    }
}
