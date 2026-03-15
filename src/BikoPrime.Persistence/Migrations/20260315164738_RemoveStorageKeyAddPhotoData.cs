using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikoPrime.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStorageKeyAddPhotoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "UserPhotos");

            migrationBuilder.RenameColumn(
                name: "IsProfilePicture",
                table: "UserPhotos",
                newName: "IsActive");

            migrationBuilder.AddColumn<long>(
                name: "AccessCount",
                table: "UserPhotos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Checksum",
                table: "UserPhotos",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "UserPhotos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "UserPhotos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileHash",
                table: "UserPhotos",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                table: "UserPhotos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "UserPhotos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_FileHash",
                table: "UserPhotos",
                column: "FileHash");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_UserId_IsActive",
                table: "UserPhotos",
                columns: new[] { "UserId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserPhotos_FileHash",
                table: "UserPhotos");

            migrationBuilder.DropIndex(
                name: "IX_UserPhotos_UserId_IsActive",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "AccessCount",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "Checksum",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "FileHash",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                table: "UserPhotos");

            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "UserPhotos");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "UserPhotos",
                newName: "IsProfilePicture");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "UserPhotos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "UserPhotos",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
