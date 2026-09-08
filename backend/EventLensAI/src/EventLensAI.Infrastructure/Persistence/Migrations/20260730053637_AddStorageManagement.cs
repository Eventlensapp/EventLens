using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "storage_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storage_folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_storage_folders_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_storage_folders_storage_folders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "storage_folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "storage_usage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedBytes = table.Column<long>(type: "bigint", nullable: false),
                    UsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileCount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_usage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "storage_files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: true),
                    StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UploaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Visibility = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScanStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsFavourite = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PreviousVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_storage_files_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_storage_files_storage_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "storage_categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_storage_files_storage_folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "storage_folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_storage_files_users_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "storage_folder_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageFolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CanWrite = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_folder_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_storage_folder_permissions_storage_folders_StorageFolderId",
                        column: x => x.StorageFolderId,
                        principalTable: "storage_folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "storage_file_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_storage_file_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_storage_file_tags_storage_files_StorageFileId",
                        column: x => x.StorageFileId,
                        principalTable: "storage_files",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "storage_categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsDeleted", "Name", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "BrandAsset", "BrandAsset", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Template", "Template", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "BoothPhoto", "BoothPhoto", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "GalleryPhoto", "GalleryPhoto", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "AIAsset", "AIAsset", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Report", "Report", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000007"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "CRMFile", "CRMFile", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "QRImage", "QRImage", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000009"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Video", "Video", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000010"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Document", "Document", null, null },
                    { new Guid("60000000-0000-0000-0000-000000000011"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Other", "Other", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_storage_categories_Type",
                table: "storage_categories",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_storage_file_tags_StorageFileId_Tag",
                table: "storage_file_tags",
                columns: new[] { "StorageFileId", "Tag" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_CategoryId",
                table: "storage_files",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_FolderId",
                table: "storage_files",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_OrganizationId",
                table: "storage_files",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_OrganizationId_CategoryId_CreatedAt",
                table: "storage_files",
                columns: new[] { "OrganizationId", "CategoryId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_OrganizationId_Checksum",
                table: "storage_files",
                columns: new[] { "OrganizationId", "Checksum" });

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_OrganizationId_FileName",
                table: "storage_files",
                columns: new[] { "OrganizationId", "FileName" });

            migrationBuilder.CreateIndex(
                name: "IX_storage_files_UploaderId",
                table: "storage_files",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_folder_permissions_StorageFolderId_UserId_RoleId",
                table: "storage_folder_permissions",
                columns: new[] { "StorageFolderId", "UserId", "RoleId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_storage_folders_OrganizationId_ParentFolderId_Name",
                table: "storage_folders",
                columns: new[] { "OrganizationId", "ParentFolderId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_storage_folders_ParentFolderId",
                table: "storage_folders",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_storage_usage_OrganizationId",
                table: "storage_usage",
                column: "OrganizationId",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "storage_file_tags");

            migrationBuilder.DropTable(
                name: "storage_folder_permissions");

            migrationBuilder.DropTable(
                name: "storage_usage");

            migrationBuilder.DropTable(
                name: "storage_files");

            migrationBuilder.DropTable(
                name: "storage_categories");

            migrationBuilder.DropTable(
                name: "storage_folders");
        }
    }
}
