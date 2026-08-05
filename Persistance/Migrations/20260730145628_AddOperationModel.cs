using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddOperationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    IdOperation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    OperationStatus = table.Column<int>(type: "int", nullable: false),
                    VideoProcess = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.IdOperation);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");
        }
    }
}
