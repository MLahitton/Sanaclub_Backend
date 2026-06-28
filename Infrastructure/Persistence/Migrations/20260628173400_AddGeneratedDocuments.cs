using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "documents");

            migrationBuilder.CreateTable(
                name: "generated_documents",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_kind = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    storage_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    generated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    generated_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_generated_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_generated_documents_patients_patient_id",
                        column: x => x.patient_id,
                        principalSchema: "clinical",
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_document_kind",
                schema: "documents",
                table: "generated_documents",
                column: "document_kind");

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_generated_at_utc",
                schema: "documents",
                table: "generated_documents",
                column: "generated_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_is_active",
                schema: "documents",
                table: "generated_documents",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_patient_id",
                schema: "documents",
                table: "generated_documents",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_source_entity_id",
                schema: "documents",
                table: "generated_documents",
                column: "source_entity_id");

            migrationBuilder.CreateIndex(
                name: "IX_generated_documents_status",
                schema: "documents",
                table: "generated_documents",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "generated_documents",
                schema: "documents");
        }
    }
}
