using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRealEstateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_real_estates_leases_lease_id",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropIndex(
                name: "ix_real_estates_lease_id",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropColumn(
                name: "lease_id",
                schema: "buildings",
                table: "real_estates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "lease_id",
                schema: "buildings",
                table: "real_estates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_lease_id",
                schema: "buildings",
                table: "real_estates",
                column: "lease_id");

            migrationBuilder.AddForeignKey(
                name: "fk_real_estates_leases_lease_id",
                schema: "buildings",
                table: "real_estates",
                column: "lease_id",
                principalSchema: "buildings",
                principalTable: "leases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
