using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Document.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "documents");

            migrationBuilder.CreateTable(
                name: "contract_templates",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "jsonb", nullable: false),
                    description = table.Column<string>(type: "jsonb", nullable: true),
                    page = table.Column<string>(type: "jsonb", nullable: false),
                    theme = table.Column<string>(type: "jsonb", nullable: false),
                    header = table.Column<string>(type: "jsonb", nullable: true),
                    footer = table.Column<string>(type: "jsonb", nullable: true),
                    bodies = table.Column<string>(type: "jsonb", nullable: false),
                    manual_fields = table.Column<string>(type: "jsonb", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    current_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    body = table.Column<string>(type: "jsonb", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_company_id = table.Column<Guid>(type: "uuid", nullable: true),
                    owner_inn = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    owner_pinfl = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    client_inn = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    client_pinfl = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    monthly_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    lease_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    lease_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contract_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    signature_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    exported_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    owner_signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    client_signed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contracts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contracts_contracts_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "documents",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract_attachments",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    object_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    content_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_attachments_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "documents",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contract_financial_items",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_financial_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_financial_items_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "documents",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contract_provider_states",
                schema: "documents",
                columns: table => new
                {
                    provider_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    sync_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    error_message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_provider_states", x => new { x.contract_id, x.provider_name });
                    table.ForeignKey(
                        name: "fk_contract_provider_states_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "documents",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contract_signing_events",
                schema: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    party = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    action = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    external_signature_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_signing_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_signing_events_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "documents",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contract_attachments_contract_id",
                schema: "documents",
                table: "contract_attachments",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_financial_items_contract_id",
                schema: "documents",
                table: "contract_financial_items",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_signing_events_contract_id",
                schema: "documents",
                table: "contract_signing_events",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_category",
                schema: "documents",
                table: "contract_templates",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_code",
                schema: "documents",
                table: "contract_templates",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_created_by_user_id",
                schema: "documents",
                table: "contract_templates",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_is_active",
                schema: "documents",
                table: "contract_templates",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_scope",
                schema: "documents",
                table: "contract_templates",
                column: "scope");

            migrationBuilder.CreateIndex(
                name: "ix_contract_templates_tenant_id",
                schema: "documents",
                table: "contract_templates",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_created_by_user_id",
                schema: "documents",
                table: "contracts",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_lease_id",
                schema: "documents",
                table: "contracts",
                column: "lease_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_parent_id",
                schema: "documents",
                table: "contracts",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_status",
                schema: "documents",
                table: "contracts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_template_id",
                schema: "documents",
                table: "contracts",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id",
                schema: "documents",
                table: "contracts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id_contract_number",
                schema: "documents",
                table: "contracts",
                columns: new[] { "tenant_id", "contract_number" },
                unique: true,
                filter: "contract_number IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract_attachments",
                schema: "documents");

            migrationBuilder.DropTable(
                name: "contract_financial_items",
                schema: "documents");

            migrationBuilder.DropTable(
                name: "contract_provider_states",
                schema: "documents");

            migrationBuilder.DropTable(
                name: "contract_signing_events",
                schema: "documents");

            migrationBuilder.DropTable(
                name: "contract_templates",
                schema: "documents");

            migrationBuilder.DropTable(
                name: "contracts",
                schema: "documents");
        }
    }
}
