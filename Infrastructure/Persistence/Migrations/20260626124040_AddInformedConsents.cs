using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInformedConsents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "informed_consents",
                schema: "clinical",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    signed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    signed_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    patient_signer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_informed_consents", x => x.id);
                    table.ForeignKey(
                        name: "FK_informed_consents_consent_statuses_consent_status_id",
                        column: x => x.consent_status_id,
                        principalSchema: "catalog",
                        principalTable: "consent_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_informed_consents_document_types_document_type_id",
                        column: x => x.document_type_id,
                        principalSchema: "catalog",
                        principalTable: "document_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_informed_consents_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_informed_consents_users_signed_by_user_id",
                        column: x => x.signed_by_user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_consent_status_id",
                schema: "clinical",
                table: "informed_consents",
                column: "consent_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_created_at_utc",
                schema: "clinical",
                table: "informed_consents",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_document_type_id",
                schema: "clinical",
                table: "informed_consents",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_is_active",
                schema: "clinical",
                table: "informed_consents",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_patient_id",
                schema: "clinical",
                table: "informed_consents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_informed_consents_signed_by_user_id",
                schema: "clinical",
                table: "informed_consents",
                column: "signed_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "informed_consents",
                schema: "clinical");
        }
    }
}
