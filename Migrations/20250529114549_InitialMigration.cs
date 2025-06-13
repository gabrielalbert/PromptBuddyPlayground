using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PromptEngineering.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicPrompts",
                columns: table => new
                {
                    DynamicPromptId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SystemPromptTemplate = table.Column<string>(type: "text", nullable: false),
                    SystemPromptKeysCommaSeperated = table.Column<string>(type: "text", nullable: false),
                    UserPromptTemplate = table.Column<string>(type: "text", nullable: false),
                    UserPromptKeysCommaSeperated = table.Column<string>(type: "text", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: false),
                    Temperature = table.Column<int>(type: "integer", nullable: true),
                    Seed = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicPrompts", x => x.DynamicPromptId);
                });

            migrationBuilder.CreateTable(
                name: "FileCollections",
                columns: table => new
                {
                    FileCollectionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileCollections", x => x.FileCollectionId);
                });

            migrationBuilder.CreateTable(
                name: "ConversionInstances",
                columns: table => new
                {
                    ConversionInstanceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    FileCollectionId = table.Column<int>(type: "integer", nullable: false),
                    CodeSplitterJSON = table.Column<string>(type: "text", nullable: true),
                    ResultFileCollectionId = table.Column<int>(type: "integer", nullable: true),
                    ConversionStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionInstances", x => x.ConversionInstanceId);
                    table.ForeignKey(
                        name: "FK_ConversionInstances_FileCollections_FileCollectionId",
                        column: x => x.FileCollectionId,
                        principalTable: "FileCollections",
                        principalColumn: "FileCollectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileEntries",
                columns: table => new
                {
                    FileEntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    FileContent = table.Column<string>(type: "text", nullable: true),
                    FileCollectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileEntries", x => x.FileEntryId);
                    table.ForeignKey(
                        name: "FK_FileEntries_FileCollections_FileCollectionId",
                        column: x => x.FileCollectionId,
                        principalTable: "FileCollections",
                        principalColumn: "FileCollectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversionInstanceLogs",
                columns: table => new
                {
                    ConversionInstanceLogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TitleText = table.Column<string>(type: "text", nullable: true),
                    DetailedText = table.Column<string>(type: "text", nullable: true),
                    LogType = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConversionInstanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionInstanceLogs", x => x.ConversionInstanceLogId);
                    table.ForeignKey(
                        name: "FK_ConversionInstanceLogs_ConversionInstances_ConversionInstan~",
                        column: x => x.ConversionInstanceId,
                        principalTable: "ConversionInstances",
                        principalColumn: "ConversionInstanceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileEntryMetadatas",
                columns: table => new
                {
                    FileEntryMetadataId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileEntryId = table.Column<int>(type: "integer", nullable: false),
                    MetadataJSON = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileEntryMetadatas", x => x.FileEntryMetadataId);
                    table.ForeignKey(
                        name: "FK_FileEntryMetadatas_FileEntries_FileEntryId",
                        column: x => x.FileEntryId,
                        principalTable: "FileEntries",
                        principalColumn: "FileEntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionInstanceLogs_ConversionInstanceId",
                table: "ConversionInstanceLogs",
                column: "ConversionInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionInstances_FileCollectionId",
                table: "ConversionInstances",
                column: "FileCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileEntries_FileCollectionId",
                table: "FileEntries",
                column: "FileCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FileEntryMetadatas_FileEntryId",
                table: "FileEntryMetadatas",
                column: "FileEntryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversionInstanceLogs");

            migrationBuilder.DropTable(
                name: "DynamicPrompts");

            migrationBuilder.DropTable(
                name: "FileEntryMetadatas");

            migrationBuilder.DropTable(
                name: "ConversionInstances");

            migrationBuilder.DropTable(
                name: "FileEntries");

            migrationBuilder.DropTable(
                name: "FileCollections");
        }
    }
}
