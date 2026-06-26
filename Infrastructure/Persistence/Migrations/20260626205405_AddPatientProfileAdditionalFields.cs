using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientProfileAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "city_or_municipality",
                schema: "clinical",
                table: "patients",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emergency_contact_name",
                schema: "clinical",
                table: "patients",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emergency_contact_phone",
                schema: "clinical",
                table: "patients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "emergency_contact_relationship",
                schema: "clinical",
                table: "patients",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "occupation",
                schema: "clinical",
                table: "patients",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "city_or_municipality",
                schema: "clinical",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "emergency_contact_name",
                schema: "clinical",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "emergency_contact_phone",
                schema: "clinical",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "emergency_contact_relationship",
                schema: "clinical",
                table: "patients");

            migrationBuilder.DropColumn(
                name: "occupation",
                schema: "clinical",
                table: "patients");
        }
    }
}
