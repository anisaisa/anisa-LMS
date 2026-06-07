using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace anisa_lms.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueStudentModuleProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ModuleProgresses_StudentId",
                table: "ModuleProgresses");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgresses_StudentId_ModuleId",
                table: "ModuleProgresses",
                columns: new[] { "StudentId", "ModuleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ModuleProgresses_StudentId_ModuleId",
                table: "ModuleProgresses");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgresses_StudentId",
                table: "ModuleProgresses",
                column: "StudentId");
        }
    }
}
