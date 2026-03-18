using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Building.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "buildings");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "amenity_categories",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenity_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    building_type = table.Column<int>(type: "integer", nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complexes",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    district_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    location = table.Column<Point>(type: "geometry", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_commercial = table.Column<bool>(type: "boolean", nullable: false),
                    is_living = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complexes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "land_categories",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_land_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "leases",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contract_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    payment_day = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_leases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "listing_categories",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    building_type = table.Column<int>(type: "integer", nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    order = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    show_in_main = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meter_types",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    icon = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meter_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "production_types",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_types",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    show_building_suggestion = table.Column<bool>(type: "boolean", nullable: false),
                    show_floor_suggestion = table.Column<bool>(type: "boolean", nullable: false),
                    can_have_units = table.Column<bool>(type: "boolean", nullable: false),
                    can_have_meters = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "renovations",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renovations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rental_purposes",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rental_purposes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room_types",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "wishlists",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listring_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_wishlists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "amenities",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenities", x => x.id);
                    table.ForeignKey(
                        name: "fk_amenities_amenity_categories_amenity_category_id",
                        column: x => x.amenity_category_id,
                        principalSchema: "buildings",
                        principalTable: "amenity_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "amenity_category_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenity_category_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_amenity_category_translates_amenity_categories_amenity_cate",
                        column: x => x.amenity_category_id,
                        principalSchema: "buildings",
                        principalTable: "amenity_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_category_translates_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "buildings",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "buildings",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    complex_id = table.Column<Guid>(type: "uuid", nullable: true),
                    region_id = table.Column<Guid>(type: "uuid", nullable: true),
                    district_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_commercial = table.Column<bool>(type: "boolean", nullable: false),
                    is_living = table.Column<bool>(type: "boolean", nullable: false),
                    total_area = table.Column<short>(type: "smallint", nullable: true),
                    floors_count = table.Column<short>(type: "smallint", nullable: true),
                    location = table.Column<Point>(type: "geometry", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_buildings", x => x.id);
                    table.ForeignKey(
                        name: "fk_buildings_complexes_complex_id",
                        column: x => x.complex_id,
                        principalSchema: "buildings",
                        principalTable: "complexes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "complex_images",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complex_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complex_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_complex_images_complexes_complex_id",
                        column: x => x.complex_id,
                        principalSchema: "buildings",
                        principalTable: "complexes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "complex_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    complex_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_complex_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_complex_translates_complexes_complex_id",
                        column: x => x.complex_id,
                        principalSchema: "buildings",
                        principalTable: "complexes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "land_category_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    land_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_land_category_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_land_category_translates_land_categories_land_category_id",
                        column: x => x.land_category_id,
                        principalSchema: "buildings",
                        principalTable: "land_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_category_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_category_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_category_translates_listing_categories_listing_cate",
                        column: x => x.listing_category_id,
                        principalSchema: "buildings",
                        principalTable: "listing_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meter_tariffs",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    meter_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: true),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    min_limit = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    max_limit = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    fixed_price = table.Column<long>(type: "bigint", nullable: true),
                    is_actual = table.Column<bool>(type: "boolean", nullable: false),
                    season = table.Column<int>(type: "integer", nullable: false),
                    social_norm_limit = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meter_tariffs", x => x.id);
                    table.ForeignKey(
                        name: "fk_meter_tariffs_meter_types_meter_type_id",
                        column: x => x.meter_type_id,
                        principalSchema: "buildings",
                        principalTable: "meter_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meter_type_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    meter_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field = table.Column<int>(type: "integer", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meter_type_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_meter_type_translates_meter_types_meter_type_id",
                        column: x => x.meter_type_id,
                        principalSchema: "buildings",
                        principalTable: "meter_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "production_type_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    production_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_production_type_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_production_type_translates_production_types_production_type",
                        column: x => x.production_type_id,
                        principalSchema: "buildings",
                        principalTable: "production_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_type_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field = table.Column<int>(type: "integer", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_type_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_type_translates_real_estate_types_real_estate_t",
                        column: x => x.real_estate_type_id,
                        principalSchema: "buildings",
                        principalTable: "real_estate_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "renovation_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    renovation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_renovation_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_renovation_translates_renovations_renovation_id",
                        column: x => x.renovation_id,
                        principalSchema: "buildings",
                        principalTable: "renovations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rental_purpose_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rental_purpose_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rental_purpose_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_rental_purpose_translates_rental_purposes_rental_purpose_id",
                        column: x => x.rental_purpose_id,
                        principalSchema: "buildings",
                        principalTable: "rental_purposes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_type_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_type_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_type_translates_room_types_room_type_id",
                        column: x => x.room_type_id,
                        principalSchema: "buildings",
                        principalTable: "room_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "amenity_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_amenity_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_amenity_translates_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalSchema: "buildings",
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "building_categories",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_building_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_building_categories_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_building_categories_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "buildings",
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "building_images",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_building_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_building_images_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "building_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_building_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_building_translates_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "floors",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    building_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<short>(type: "smallint", nullable: false),
                    total_area = table.Column<float>(type: "real", nullable: true),
                    ceiling_height = table.Column<float>(type: "real", nullable: true),
                    plan = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_floors", x => x.id);
                    table.ForeignKey(
                        name: "fk_floors_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    land_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    production_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    renovation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cadastral_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    building_id = table.Column<Guid>(type: "uuid", nullable: true),
                    building_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    floor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    floor_number = table.Column<short>(type: "smallint", nullable: true),
                    total_floors = table.Column<short>(type: "smallint", nullable: true),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_area = table.Column<float>(type: "real", nullable: true),
                    living_area = table.Column<float>(type: "real", nullable: true),
                    ceiling_height = table.Column<float>(type: "real", nullable: true),
                    rooms_count = table.Column<int>(type: "integer", nullable: true),
                    region_id = table.Column<Guid>(type: "uuid", nullable: true),
                    district_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location = table.Column<Point>(type: "geometry", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    plan = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    moderation_status = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    property_category = table.Column<int>(type: "integer", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estates", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estates_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estates_floors_floor_id",
                        column: x => x.floor_id,
                        principalSchema: "buildings",
                        principalTable: "floors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estates_land_categories_land_category_id",
                        column: x => x.land_category_id,
                        principalSchema: "buildings",
                        principalTable: "land_categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estates_leases_lease_id",
                        column: x => x.lease_id,
                        principalSchema: "buildings",
                        principalTable: "leases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_real_estates_production_types_production_type_id",
                        column: x => x.production_type_id,
                        principalSchema: "buildings",
                        principalTable: "production_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estates_real_estate_types_real_estate_type_id",
                        column: x => x.real_estate_type_id,
                        principalSchema: "buildings",
                        principalTable: "real_estate_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_real_estates_renovations_renovation_id",
                        column: x => x.renovation_id,
                        principalSchema: "buildings",
                        principalTable: "renovations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lease_items",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    monthly_rent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    deposit_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_meters_included = table.Column<bool>(type: "boolean", nullable: false),
                    meter_ids = table.Column<Guid[]>(type: "uuid[]", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lease_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_lease_items_leases_lease_id",
                        column: x => x.lease_id,
                        principalSchema: "buildings",
                        principalTable: "leases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lease_items_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listings",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    renovation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category_ids = table.Column<string>(type: "text", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    complex_id = table.Column<Guid>(type: "uuid", nullable: true),
                    complex_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    building_id = table.Column<Guid>(type: "uuid", nullable: true),
                    building_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    floor_ids = table.Column<string>(type: "text", nullable: true),
                    room_ids = table.Column<string>(type: "text", nullable: true),
                    unit_ids = table.Column<string>(type: "text", nullable: true),
                    floor_numbers = table.Column<string>(type: "text", nullable: true),
                    rooms_count = table.Column<int>(type: "integer", nullable: true),
                    living_area = table.Column<float>(type: "real", nullable: true),
                    total_area = table.Column<float>(type: "real", nullable: false),
                    ceiling_height = table.Column<float>(type: "real", nullable: true),
                    price_for_month = table.Column<long>(type: "bigint", nullable: true),
                    price_per_square_meter = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    rental_purpose_id = table.Column<Guid>(type: "uuid", nullable: true),
                    min_lease_term = table.Column<int>(type: "integer", nullable: true),
                    utility_payment_type = table.Column<int>(type: "integer", nullable: true),
                    region_id = table.Column<Guid>(type: "uuid", nullable: true),
                    district_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location = table.Column<Point>(type: "geometry", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    next_available_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    moderation_status = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listings", x => x.id);
                    table.ForeignKey(
                        name: "fk_listings_buildings_building_id",
                        column: x => x.building_id,
                        principalSchema: "buildings",
                        principalTable: "buildings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_listings_complexes_complex_id",
                        column: x => x.complex_id,
                        principalSchema: "buildings",
                        principalTable: "complexes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_listings_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listings_renovations_renovation_id",
                        column: x => x.renovation_id,
                        principalSchema: "buildings",
                        principalTable: "renovations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_listings_rental_purposes_rental_purpose_id",
                        column: x => x.rental_purpose_id,
                        principalSchema: "buildings",
                        principalTable: "rental_purposes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "meters",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    meter_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    installation_date = table.Column<DateOnly>(type: "date", nullable: true),
                    verification_date = table.Column<DateOnly>(type: "date", nullable: true),
                    next_verification_date = table.Column<DateOnly>(type: "date", nullable: true),
                    initial_reading = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meters", x => x.id);
                    table.ForeignKey(
                        name: "fk_meters_meter_types_meter_type_id",
                        column: x => x.meter_type_id,
                        principalSchema: "buildings",
                        principalTable: "meter_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_meters_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_amenities",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_amenities", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalSchema: "buildings",
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_real_estate_amenities_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_images",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_images_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_translates_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    floor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    area = table.Column<float>(type: "real", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                    table.ForeignKey(
                        name: "fk_rooms_floors_floor_id",
                        column: x => x.floor_id,
                        principalSchema: "buildings",
                        principalTable: "floors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_rooms_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rooms_room_types_room_type_id",
                        column: x => x.room_type_id,
                        principalSchema: "buildings",
                        principalTable: "room_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_amenities",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amenity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_amenities", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_amenities_amenities_amenity_id",
                        column: x => x.amenity_id,
                        principalSchema: "buildings",
                        principalTable: "amenities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_listing_amenities_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "buildings",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_requests",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_requests_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "buildings",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "listing_translates",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    listing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_id = table.Column<Guid>(type: "uuid", nullable: false),
                    language_short_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_listing_translates", x => x.id);
                    table.ForeignKey(
                        name: "fk_listing_translates_listings_listing_id",
                        column: x => x.listing_id,
                        principalSchema: "buildings",
                        principalTable: "listings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "meter_readings",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    meter_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reading_date = table.Column<DateOnly>(type: "date", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    previous_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    consumption = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_manual = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_meter_readings", x => x.id);
                    table.ForeignKey(
                        name: "fk_meter_readings_meters_meter_id",
                        column: x => x.meter_id,
                        principalSchema: "buildings",
                        principalTable: "meters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_units",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: true),
                    real_estate_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    floor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    room_id = table.Column<Guid>(type: "uuid", nullable: true),
                    renovation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    floor_number = table.Column<short>(type: "smallint", nullable: true),
                    room_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_area = table.Column<float>(type: "real", nullable: true),
                    ceiling_height = table.Column<float>(type: "real", nullable: true),
                    coordinates = table.Column<string>(type: "jsonb", nullable: true),
                    plan = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    images = table.Column<List<string>>(type: "text[]", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    moderation_status = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_units_floors_floor_id",
                        column: x => x.floor_id,
                        principalSchema: "buildings",
                        principalTable: "floors",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estate_units_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estate_units_renovations_renovation_id",
                        column: x => x.renovation_id,
                        principalSchema: "buildings",
                        principalTable: "renovations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estate_units_rooms_room_id",
                        column: x => x.room_id,
                        principalSchema: "buildings",
                        principalTable: "rooms",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "communal_bills",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meter_reading_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meter_tariff_id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    billing_period_end = table.Column<DateOnly>(type: "date", nullable: false),
                    consumption = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_amount = table.Column<long>(type: "bigint", nullable: false),
                    fixed_amount = table.Column<long>(type: "bigint", nullable: true),
                    paid_amount = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communal_bills", x => x.id);
                    table.ForeignKey(
                        name: "fk_communal_bills_meter_readings_meter_reading_id",
                        column: x => x.meter_reading_id,
                        principalSchema: "buildings",
                        principalTable: "meter_readings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_communal_bills_meter_tariffs_meter_tariff_id",
                        column: x => x.meter_tariff_id,
                        principalSchema: "buildings",
                        principalTable: "meter_tariffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_communal_bills_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "real_estate_delegations",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    real_estate_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: true),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: true),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contract_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    commission_percent = table.Column<long>(type: "bigint", nullable: true),
                    commission_fixed = table.Column<long>(type: "bigint", nullable: true),
                    max_lease_months = table.Column<short>(type: "smallint", nullable: true),
                    max_lease_amount = table.Column<long>(type: "bigint", nullable: true),
                    require_owner_approval = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_real_estate_delegations", x => x.id);
                    table.ForeignKey(
                        name: "fk_real_estate_delegations_real_estate_units_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "buildings",
                        principalTable: "real_estate_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_real_estate_delegations_real_estates_real_estate_id",
                        column: x => x.real_estate_id,
                        principalSchema: "buildings",
                        principalTable: "real_estates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "communal_payments",
                schema: "buildings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    communal_bill_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    payment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communal_payments", x => x.id);
                    table.ForeignKey(
                        name: "fk_communal_payments_communal_bills_communal_bill_id",
                        column: x => x.communal_bill_id,
                        principalSchema: "buildings",
                        principalTable: "communal_bills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_amenities_amenity_category_id",
                schema: "buildings",
                table: "amenities",
                column: "amenity_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_amenity_categories_id",
                schema: "buildings",
                table: "amenity_categories",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_amenity_category_translates_amenity_category_id",
                schema: "buildings",
                table: "amenity_category_translates",
                column: "amenity_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_amenity_category_translates_language_short_code",
                schema: "buildings",
                table: "amenity_category_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_amenity_translates_amenity_id",
                schema: "buildings",
                table: "amenity_translates",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "ix_amenity_translates_language_short_code",
                schema: "buildings",
                table: "amenity_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_building_categories_building_id",
                schema: "buildings",
                table: "building_categories",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_building_categories_category_id",
                schema: "buildings",
                table: "building_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_building_images_building_id",
                schema: "buildings",
                table: "building_images",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_building_translates_building_id",
                schema: "buildings",
                table: "building_translates",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_building_translates_language_short_code",
                schema: "buildings",
                table: "building_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_buildings_complex_id",
                schema: "buildings",
                table: "buildings",
                column: "complex_id");

            migrationBuilder.CreateIndex(
                name: "ix_buildings_id",
                schema: "buildings",
                table: "buildings",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_categories_id",
                schema: "buildings",
                table: "categories",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_category_translates_category_id",
                schema: "buildings",
                table: "category_translates",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_category_translates_language_short_code",
                schema: "buildings",
                table: "category_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_communal_bills_meter_reading_id",
                schema: "buildings",
                table: "communal_bills",
                column: "meter_reading_id");

            migrationBuilder.CreateIndex(
                name: "ix_communal_bills_meter_tariff_id",
                schema: "buildings",
                table: "communal_bills",
                column: "meter_tariff_id");

            migrationBuilder.CreateIndex(
                name: "ix_communal_bills_real_estate_id",
                schema: "buildings",
                table: "communal_bills",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_communal_bills_status",
                schema: "buildings",
                table: "communal_bills",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_communal_payments_communal_bill_id",
                schema: "buildings",
                table: "communal_payments",
                column: "communal_bill_id");

            migrationBuilder.CreateIndex(
                name: "ix_complex_images_complex_id",
                schema: "buildings",
                table: "complex_images",
                column: "complex_id");

            migrationBuilder.CreateIndex(
                name: "ix_complex_translates_complex_id",
                schema: "buildings",
                table: "complex_translates",
                column: "complex_id");

            migrationBuilder.CreateIndex(
                name: "ix_complex_translates_language_short_code",
                schema: "buildings",
                table: "complex_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_complexes_is_active",
                schema: "buildings",
                table: "complexes",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_complexes_is_commercial",
                schema: "buildings",
                table: "complexes",
                column: "is_commercial");

            migrationBuilder.CreateIndex(
                name: "ix_complexes_is_living",
                schema: "buildings",
                table: "complexes",
                column: "is_living");

            migrationBuilder.CreateIndex(
                name: "ix_complexes_name",
                schema: "buildings",
                table: "complexes",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_floors_building_id",
                schema: "buildings",
                table: "floors",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_floors_id",
                schema: "buildings",
                table: "floors",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_floors_number",
                schema: "buildings",
                table: "floors",
                column: "number");

            migrationBuilder.CreateIndex(
                name: "ix_land_category_translates_land_category_id",
                schema: "buildings",
                table: "land_category_translates",
                column: "land_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_lease_items_id",
                schema: "buildings",
                table: "lease_items",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lease_items_lease_id",
                schema: "buildings",
                table: "lease_items",
                column: "lease_id");

            migrationBuilder.CreateIndex(
                name: "ix_lease_items_listing_id",
                schema: "buildings",
                table: "lease_items",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_lease_items_real_estate_id",
                schema: "buildings",
                table: "lease_items",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_leases_agent_id_client_id",
                schema: "buildings",
                table: "leases",
                columns: new[] { "agent_id", "client_id" });

            migrationBuilder.CreateIndex(
                name: "ix_leases_id",
                schema: "buildings",
                table: "leases",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_leases_owner_id_client_id",
                schema: "buildings",
                table: "leases",
                columns: new[] { "owner_id", "client_id" });

            migrationBuilder.CreateIndex(
                name: "ix_listing_amenities_amenity_id",
                schema: "buildings",
                table: "listing_amenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_amenities_listing_id",
                schema: "buildings",
                table: "listing_amenities",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_categories_id",
                schema: "buildings",
                table: "listing_categories",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_category_translates_language_short_code",
                schema: "buildings",
                table: "listing_category_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_listing_category_translates_listing_category_id",
                schema: "buildings",
                table: "listing_category_translates",
                column: "listing_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_requests_client_id",
                schema: "buildings",
                table: "listing_requests",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_requests_id",
                schema: "buildings",
                table: "listing_requests",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listing_requests_listing_id",
                schema: "buildings",
                table: "listing_requests",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_requests_owner_id",
                schema: "buildings",
                table: "listing_requests",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_listing_translates_language_short_code",
                schema: "buildings",
                table: "listing_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_listing_translates_listing_id",
                schema: "buildings",
                table: "listing_translates",
                column: "listing_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_building_id",
                schema: "buildings",
                table: "listings",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_complex_id",
                schema: "buildings",
                table: "listings",
                column: "complex_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_id",
                schema: "buildings",
                table: "listings",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_listings_owner_id",
                schema: "buildings",
                table: "listings",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_real_estate_id",
                schema: "buildings",
                table: "listings",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_renovation_id",
                schema: "buildings",
                table: "listings",
                column: "renovation_id");

            migrationBuilder.CreateIndex(
                name: "ix_listings_rental_purpose_id",
                schema: "buildings",
                table: "listings",
                column: "rental_purpose_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_readings_meter_id",
                schema: "buildings",
                table: "meter_readings",
                column: "meter_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_readings_real_estate_id",
                schema: "buildings",
                table: "meter_readings",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_tariffs_meter_type_id",
                schema: "buildings",
                table: "meter_tariffs",
                column: "meter_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_type_translates_field",
                schema: "buildings",
                table: "meter_type_translates",
                column: "field");

            migrationBuilder.CreateIndex(
                name: "ix_meter_type_translates_language_short_code",
                schema: "buildings",
                table: "meter_type_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_meter_type_translates_meter_type_id",
                schema: "buildings",
                table: "meter_type_translates",
                column: "meter_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_meter_types_id",
                schema: "buildings",
                table: "meter_types",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_meters_id",
                schema: "buildings",
                table: "meters",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_meters_meter_type_id",
                schema: "buildings",
                table: "meters",
                column: "meter_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_meters_real_estate_id",
                schema: "buildings",
                table: "meters",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_meters_serial_number",
                schema: "buildings",
                table: "meters",
                column: "serial_number");

            migrationBuilder.CreateIndex(
                name: "ix_production_type_translates_production_type_id",
                schema: "buildings",
                table: "production_type_translates",
                column: "production_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_amenities_amenity_id",
                schema: "buildings",
                table: "real_estate_amenities",
                column: "amenity_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_amenities_real_estate_id_amenity_id",
                schema: "buildings",
                table: "real_estate_amenities",
                columns: new[] { "real_estate_id", "amenity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_delegations_owner_id_agent_id",
                schema: "buildings",
                table: "real_estate_delegations",
                columns: new[] { "owner_id", "agent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_delegations_real_estate_id",
                schema: "buildings",
                table: "real_estate_delegations",
                column: "real_estate_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_delegations_unit_id",
                schema: "buildings",
                table: "real_estate_delegations",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_images_real_estate_id",
                schema: "buildings",
                table: "real_estate_images",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_translates_language_short_code",
                schema: "buildings",
                table: "real_estate_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_translates_real_estate_id",
                schema: "buildings",
                table: "real_estate_translates",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_type_translates_field",
                schema: "buildings",
                table: "real_estate_type_translates",
                column: "field");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_type_translates_language_short_code",
                schema: "buildings",
                table: "real_estate_type_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_type_translates_real_estate_type_id",
                schema: "buildings",
                table: "real_estate_type_translates",
                column: "real_estate_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_types_can_have_meters",
                schema: "buildings",
                table: "real_estate_types",
                column: "can_have_meters");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_types_can_have_units",
                schema: "buildings",
                table: "real_estate_types",
                column: "can_have_units");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_types_id",
                schema: "buildings",
                table: "real_estate_types",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_floor_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_owner_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_real_estate_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_real_estate_type_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "real_estate_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_renovation_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "renovation_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estate_units_room_id",
                schema: "buildings",
                table: "real_estate_units",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_building_id",
                schema: "buildings",
                table: "real_estates",
                column: "building_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_district_id",
                schema: "buildings",
                table: "real_estates",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_floor_id",
                schema: "buildings",
                table: "real_estates",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_id",
                schema: "buildings",
                table: "real_estates",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_land_category_id",
                schema: "buildings",
                table: "real_estates",
                column: "land_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_lease_id",
                schema: "buildings",
                table: "real_estates",
                column: "lease_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_production_type_id",
                schema: "buildings",
                table: "real_estates",
                column: "production_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_real_estate_type_id",
                schema: "buildings",
                table: "real_estates",
                column: "real_estate_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_region_id",
                schema: "buildings",
                table: "real_estates",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_renovation_id",
                schema: "buildings",
                table: "real_estates",
                column: "renovation_id");

            migrationBuilder.CreateIndex(
                name: "ix_renovation_translates_language_short_code",
                schema: "buildings",
                table: "renovation_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_renovation_translates_renovation_id",
                schema: "buildings",
                table: "renovation_translates",
                column: "renovation_id");

            migrationBuilder.CreateIndex(
                name: "ix_renovation_translates_value",
                schema: "buildings",
                table: "renovation_translates",
                column: "value");

            migrationBuilder.CreateIndex(
                name: "ix_renovations_is_active",
                schema: "buildings",
                table: "renovations",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_rental_purpose_translates_language_short_code",
                schema: "buildings",
                table: "rental_purpose_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_rental_purpose_translates_rental_purpose_id",
                schema: "buildings",
                table: "rental_purpose_translates",
                column: "rental_purpose_id");

            migrationBuilder.CreateIndex(
                name: "ix_rental_purposes_id",
                schema: "buildings",
                table: "rental_purposes",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_type_translates_language_short_code",
                schema: "buildings",
                table: "room_type_translates",
                column: "language_short_code");

            migrationBuilder.CreateIndex(
                name: "ix_room_type_translates_room_type_id",
                schema: "buildings",
                table: "room_type_translates",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_type_translates_value",
                schema: "buildings",
                table: "room_type_translates",
                column: "value");

            migrationBuilder.CreateIndex(
                name: "ix_room_types_id",
                schema: "buildings",
                table: "room_types",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_floor_id",
                schema: "buildings",
                table: "rooms",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_real_estate_id",
                schema: "buildings",
                table: "rooms",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_room_type_id",
                schema: "buildings",
                table: "rooms",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_wishlists_tenant_id_user_id",
                schema: "buildings",
                table: "wishlists",
                columns: new[] { "tenant_id", "user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "amenity_category_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "amenity_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "building_categories",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "building_images",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "building_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "category_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "communal_payments",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "complex_images",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "complex_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "land_category_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "lease_items",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listing_amenities",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listing_category_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listing_requests",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listing_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "meter_type_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "production_type_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_amenities",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_delegations",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_images",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_type_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "renovation_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "rental_purpose_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "room_type_translates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "wishlists",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "communal_bills",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listing_categories",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "listings",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "amenities",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_units",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "meter_readings",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "meter_tariffs",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "rental_purposes",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "amenity_categories",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "rooms",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "meters",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "room_types",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "meter_types",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estates",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "floors",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "land_categories",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "leases",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "production_types",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "real_estate_types",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "renovations",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "buildings",
                schema: "buildings");

            migrationBuilder.DropTable(
                name: "complexes",
                schema: "buildings");
        }
    }
}
