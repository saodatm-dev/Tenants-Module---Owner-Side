using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Didox.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "didox");

            migrationBuilder.CreateTable(
                name: "didox_account",
                schema: "didox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    login = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    password = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pinfl = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    tin = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_didox_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "didox_tokens",
                schema: "didox",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_in = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_didox_tokens", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_didox_account_owner_id",
                schema: "didox",
                table: "didox_account",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_didox_account_pinfl",
                schema: "didox",
                table: "didox_account",
                column: "pinfl");

            migrationBuilder.CreateIndex(
                name: "ix_didox_account_tin",
                schema: "didox",
                table: "didox_account",
                column: "tin");

            migrationBuilder.CreateIndex(
                name: "ix_didox_tokens_owner_id",
                schema: "didox",
                table: "didox_tokens",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "didox_account",
                schema: "didox");

            migrationBuilder.DropTable(
                name: "didox_tokens",
                schema: "didox");
        }
    }
}
