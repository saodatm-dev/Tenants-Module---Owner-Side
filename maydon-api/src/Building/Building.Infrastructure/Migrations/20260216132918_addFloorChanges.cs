using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Building.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFloorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_floors_buildings_building_id",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropForeignKey(
                name: "fk_real_estates_floors_floor_id",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.AddColumn<short>(
                name: "above_floors",
                schema: "buildings",
                table: "real_estates",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "below_floors",
                schema: "buildings",
                table: "real_estates",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "floor_id1",
                schema: "buildings",
                table: "real_estates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "can_have_floors",
                schema: "buildings",
                table: "real_estate_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "building_id",
                schema: "buildings",
                table: "floors",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "label",
                schema: "buildings",
                table: "floors",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "real_estate_id",
                schema: "buildings",
                table: "floors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                schema: "buildings",
                table: "floors",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_real_estates_floor_id1",
                schema: "buildings",
                table: "real_estates",
                column: "floor_id1");

            migrationBuilder.CreateIndex(
                name: "ix_floors_real_estate_id",
                schema: "buildings",
                table: "floors",
                column: "real_estate_id");

            migrationBuilder.CreateIndex(
                name: "ix_floors_type",
                schema: "buildings",
                table: "floors",
                column: "type");

            migrationBuilder.AddForeignKey(
                name: "fk_floors_buildings_building_id",
                schema: "buildings",
                table: "floors",
                column: "building_id",
                principalSchema: "buildings",
                principalTable: "buildings",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_floors_real_estates_real_estate_id",
                schema: "buildings",
                table: "floors",
                column: "real_estate_id",
                principalSchema: "buildings",
                principalTable: "real_estates",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_real_estates_floors_floor_id",
                schema: "buildings",
                table: "real_estates",
                column: "floor_id",
                principalSchema: "buildings",
                principalTable: "floors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_real_estates_floors_floor_id1",
                schema: "buildings",
                table: "real_estates",
                column: "floor_id1",
                principalSchema: "buildings",
                principalTable: "floors",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_floors_buildings_building_id",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropForeignKey(
                name: "fk_floors_real_estates_real_estate_id",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropForeignKey(
                name: "fk_real_estates_floors_floor_id",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropForeignKey(
                name: "fk_real_estates_floors_floor_id1",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropIndex(
                name: "ix_real_estates_floor_id1",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropIndex(
                name: "ix_floors_real_estate_id",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropIndex(
                name: "ix_floors_type",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropColumn(
                name: "above_floors",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropColumn(
                name: "below_floors",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropColumn(
                name: "floor_id1",
                schema: "buildings",
                table: "real_estates");

            migrationBuilder.DropColumn(
                name: "can_have_floors",
                schema: "buildings",
                table: "real_estate_types");

            migrationBuilder.DropColumn(
                name: "label",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropColumn(
                name: "real_estate_id",
                schema: "buildings",
                table: "floors");

            migrationBuilder.DropColumn(
                name: "type",
                schema: "buildings",
                table: "floors");

            migrationBuilder.AlterColumn<Guid>(
                name: "building_id",
                schema: "buildings",
                table: "floors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_floors_buildings_building_id",
                schema: "buildings",
                table: "floors",
                column: "building_id",
                principalSchema: "buildings",
                principalTable: "buildings",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_real_estates_floors_floor_id",
                schema: "buildings",
                table: "real_estates",
                column: "floor_id",
                principalSchema: "buildings",
                principalTable: "floors",
                principalColumn: "id");
        }
    }
}
