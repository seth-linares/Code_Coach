using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeniorProjBackend.Migrations
{
    /// <inheritdoc />
    public partial class mssqlonprem_migration_764 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RankIconURL",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RankIconURL",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "path/to/default/rank-icon.jpg");
        }
    }
}
