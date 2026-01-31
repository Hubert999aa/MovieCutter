using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    IdProfile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.IdProfile);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    IdSource = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    IdProfile = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.IdSource);
                    table.ForeignKey(
                        name: "FK_Sources_Profiles_IdProfile",
                        column: x => x.IdProfile,
                        principalTable: "Profiles",
                        principalColumn: "IdProfile");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sources_IdProfile",
                table: "Sources",
                column: "IdProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
