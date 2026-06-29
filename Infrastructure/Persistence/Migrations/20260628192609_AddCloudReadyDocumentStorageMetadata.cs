using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudReadyDocumentStorageMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "storage_bucket",
                schema: "documents",
                table: "generated_documents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_external_id",
                schema: "documents",
                table: "generated_documents",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_object_key",
                schema: "documents",
                table: "generated_documents",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "storage_provider",
                schema: "documents",
                table: "generated_documents",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "storage_bucket",
                schema: "documents",
                table: "generated_documents");

            migrationBuilder.DropColumn(
                name: "storage_external_id",
                schema: "documents",
                table: "generated_documents");

            migrationBuilder.DropColumn(
                name: "storage_object_key",
                schema: "documents",
                table: "generated_documents");

            migrationBuilder.DropColumn(
                name: "storage_provider",
                schema: "documents",
                table: "generated_documents");
        }
    }
}
