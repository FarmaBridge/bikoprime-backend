using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikoPrime.Persistence.Migrations;

/// <inheritdoc />
public partial class RefactorPhotosWithSecurityFields : Migration
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

        migrationBuilder.DropColumn(
            name: "IsProfilePicture",
            table: "UserPhotos");

        migrationBuilder.AddColumn<string>(
            name: "StorageKey",
            table: "UserPhotos",
            type: "text",
            nullable: false,
            defaultValue: "",
            comment: "UUID único para localizar arquivo no storage (NUNCA expor ao cliente)");

        migrationBuilder.AddColumn<string>(
            name: "FileHash",
            table: "UserPhotos",
            type: "text",
            nullable: false,
            defaultValue: "",
            comment: "SHA256 hash do arquivo para deduplicação");

        migrationBuilder.AddColumn<string>(
            name: "Checksum",
            table: "UserPhotos",
            type: "text",
            nullable: false,
            defaultValue: "",
            comment: "Checksum SHA256 para validação de integridade");

        migrationBuilder.AddColumn<bool>(
            name: "IsActive",
            table: "UserPhotos",
            type: "boolean",
            nullable: false,
            defaultValue: true,
            comment: "Se essa é a foto ativa do usuário (soft delete)");

        migrationBuilder.AddColumn<DateTime>(
            name: "DeactivatedAt",
            table: "UserPhotos",
            type: "timestamp with time zone",
            nullable: true,
            comment: "Quando a foto foi deativada (soft delete)");

        migrationBuilder.AddColumn<string>(
            name: "DeactivationReason",
            table: "UserPhotos",
            type: "text",
            nullable: true,
            comment: "Motivo da deativação (user_replaced_photo, user_deleted, admin_removed)");

        migrationBuilder.AddColumn<long>(
            name: "AccessCount",
            table: "UserPhotos",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            comment: "Contador de acessos para analytics");

        migrationBuilder.AddColumn<DateTime>(
            name: "LastAccessedAt",
            table: "UserPhotos",
            type: "timestamp with time zone",
            nullable: true,
            comment: "Última vez que essa foto foi acessada");

        // Criar índice para queries otimizadas
        migrationBuilder.CreateIndex(
            name: "IX_UserPhotos_UserId_IsActive",
            table: "UserPhotos",
            columns: new[] { "UserId", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_UserPhotos_FileHash",
            table: "UserPhotos",
            column: "FileHash");

        migrationBuilder.CreateIndex(
            name: "IX_UserPhotos_StorageKey",
            table: "UserPhotos",
            column: "StorageKey",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_UserPhotos_UserId_IsActive",
            table: "UserPhotos");

        migrationBuilder.DropIndex(
            name: "IX_UserPhotos_FileHash",
            table: "UserPhotos");

        migrationBuilder.DropIndex(
            name: "IX_UserPhotos_StorageKey",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "StorageKey",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "FileHash",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "Checksum",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "IsActive",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "DeactivatedAt",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "DeactivationReason",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "AccessCount",
            table: "UserPhotos");

        migrationBuilder.DropColumn(
            name: "LastAccessedAt",
            table: "UserPhotos");

        migrationBuilder.AddColumn<string>(
            name: "FileExtension",
            table: "UserPhotos",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "FilePath",
            table: "UserPhotos",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "IsProfilePicture",
            table: "UserPhotos",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }
}
