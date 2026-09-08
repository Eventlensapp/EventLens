using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventLensAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionEntitlements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entitlement_usage_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Metric = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entitlement_usage_records", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "feature_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feature_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    MaximumEvents = table.Column<int>(type: "int", nullable: false),
                    MaximumTeamMembers = table.Column<int>(type: "int", nullable: false),
                    MaximumStorage = table.Column<long>(type: "bigint", nullable: false),
                    MaximumGalleries = table.Column<int>(type: "int", nullable: false),
                    MaximumAIJobs = table.Column<int>(type: "int", nullable: false),
                    MaximumTemplates = table.Column<int>(type: "int", nullable: false),
                    MaximumUploads = table.Column<int>(type: "int", nullable: false),
                    MaximumMonthlyCaptures = table.Column<int>(type: "int", nullable: false),
                    MaximumApiRequests = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "organization_subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrialEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_organization_subscriptions_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_organization_subscriptions_subscription_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "plan_features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_plan_features_feature_definitions_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "feature_definitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_plan_features_subscription_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "feature_definitions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsDeleted", "Key", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("71000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to unlimited.events.", false, "unlimited.events", "Unlimited Events", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to unlimited.storage.", false, "unlimited.storage", "Unlimited Storage", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to ai.studio.", false, "ai.studio", "Ai Studio", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to gallery.", false, "gallery", "Gallery", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to printing.", false, "printing", "Printing", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to qr.gallery.", false, "qr.gallery", "Qr Gallery", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000007"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to crm.", false, "crm", "Crm", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to marketing.", false, "marketing", "Marketing", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000009"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to analytics.", false, "analytics", "Analytics", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000010"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to template.studio.", false, "template.studio", "Template Studio", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000011"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to api.access.", false, "api.access", "Api Access", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000012"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to white.label.", false, "white.label", "White Label", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000013"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to custom.domain.", false, "custom.domain", "Custom Domain", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000014"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to live.slideshow.", false, "live.slideshow", "Live Slideshow", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000015"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to booth.video.", false, "booth.video", "Booth Video", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000016"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to booth.gif.", false, "booth.gif", "Booth Gif", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000017"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to booth.boomerang.", false, "booth.boomerang", "Booth Boomerang", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000018"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to dslr.support.", false, "dslr.support", "Dslr Support", null, null },
                    { new Guid("71000000-0000-0000-0000-000000000019"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Controls access to priority.support.", false, "priority.support", "Priority Support", null, null }
                });

            migrationBuilder.InsertData(
                table: "subscription_plans",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "DisplayOrder", "IsActive", "IsDeleted", "MaximumAIJobs", "MaximumApiRequests", "MaximumEvents", "MaximumGalleries", "MaximumMonthlyCaptures", "MaximumStorage", "MaximumTeamMembers", "MaximumTemplates", "MaximumUploads", "MonthlyPrice", "Name", "UpdatedAt", "UpdatedBy", "YearlyPrice" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), "Free", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Essential event tools", 1, true, false, 10, 1000, 3, 1, 500, 1073741824L, 1, 5, 100, 0m, "Free", null, null, 0m },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "Creator", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "For individual creators", 2, true, false, 250, 10000, 25, 25, 10000, 53687091200L, 3, 50, 5000, 19m, "Creator", null, null, 190m },
                    { new Guid("70000000-0000-0000-0000-000000000003"), "Professional", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "For professional studios", 3, true, false, 1000, 100000, 100, 100, 50000, 214748364800L, 10, 250, 25000, 49m, "Professional", null, null, 490m },
                    { new Guid("70000000-0000-0000-0000-000000000004"), "Business", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "For growing agencies", 4, true, false, 2500, 1000000, 500, 1000, 250000, 536870912000L, 25, 1000, 100000, 79m, "Business", null, null, 790m },
                    { new Guid("70000000-0000-0000-0000-000000000005"), "Enterprise", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Enterprise scale and governance", 5, true, false, 25000, 10000000, 10000, 100000, 5000000, 2199023255552L, 1000, 10000, 1000000, 299m, "Enterprise", null, null, 2990m },
                    { new Guid("70000000-0000-0000-0000-000000000006"), "CustomEnterprise", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Contract-defined limits", 6, true, false, -1, -1, -1, -1, -1, -1L, -1, -1, -1, 0m, "Custom Enterprise", null, null, 0m }
                });

            migrationBuilder.InsertData(
                table: "plan_features",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Enabled", "FeatureId", "IsDeleted", "PlanId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("72000000-0000-0000-0000-000000000001"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000002"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000003"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000004"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000005"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000006"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000007"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000008"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000009"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000010"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000011"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000012"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000013"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000014"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000015"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000016"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000017"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000018"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000019"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000001"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000020"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000021"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000022"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000023"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000024"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000025"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000026"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000027"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000028"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000029"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000030"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000031"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000032"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000033"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000034"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000035"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000036"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000037"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000038"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000002"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000039"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000040"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000041"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000042"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000043"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000044"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000045"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000046"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000047"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000048"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000049"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000050"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000051"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000052"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000053"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000054"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000055"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000056"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000057"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000003"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000058"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000059"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000060"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000061"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000062"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000063"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000064"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000065"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000066"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000067"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000068"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000069"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000070"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000071"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000072"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000073"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000074"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000075"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000076"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000004"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000077"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000078"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000079"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000080"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000081"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000082"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000083"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000084"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000085"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000086"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000087"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000088"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000089"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000090"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000091"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000092"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000093"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000094"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000095"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000005"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000096"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000001"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000097"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000002"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000098"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000003"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000099"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000004"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000100"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000005"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000101"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000006"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000102"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000007"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000103"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000008"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000104"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000009"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000105"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000010"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000106"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000011"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000107"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000012"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000108"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000013"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000109"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000014"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000110"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000015"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000111"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000016"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000112"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000017"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000113"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000018"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null },
                    { new Guid("72000000-0000-0000-0000-000000000114"), new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new Guid("71000000-0000-0000-0000-000000000019"), false, new Guid("70000000-0000-0000-0000-000000000006"), null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_entitlement_usage_records_OrganizationId_Metric_OccurredAt",
                table: "entitlement_usage_records",
                columns: new[] { "OrganizationId", "Metric", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_entitlement_usage_records_OrganizationId_Reference",
                table: "entitlement_usage_records",
                columns: new[] { "OrganizationId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_feature_definitions_Key",
                table: "feature_definitions",
                column: "Key",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_organization_subscriptions_OrganizationId_Status",
                table: "organization_subscriptions",
                columns: new[] { "OrganizationId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_organization_subscriptions_PlanId",
                table: "organization_subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_features_FeatureId",
                table: "plan_features",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_features_PlanId_FeatureId",
                table: "plan_features",
                columns: new[] { "PlanId", "FeatureId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_Code",
                table: "subscription_plans",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_plans_IsActive_DisplayOrder",
                table: "subscription_plans",
                columns: new[] { "IsActive", "DisplayOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entitlement_usage_records");

            migrationBuilder.DropTable(
                name: "organization_subscriptions");

            migrationBuilder.DropTable(
                name: "plan_features");

            migrationBuilder.DropTable(
                name: "feature_definitions");

            migrationBuilder.DropTable(
                name: "subscription_plans");
        }
    }
}
