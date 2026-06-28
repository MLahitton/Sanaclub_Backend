using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sanaclub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTreatmentSheetAutoApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "approved_at_utc",
                schema: "clinical",
                table: "treatment_sheets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "approved_by_user_id",
                schema: "clinical",
                table: "treatment_sheets",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approved_at_utc",
                schema: "clinical",
                table: "treatment_sheets");

            migrationBuilder.DropColumn(
                name: "approved_by_user_id",
                schema: "clinical",
                table: "treatment_sheets");
        }
    }
}
