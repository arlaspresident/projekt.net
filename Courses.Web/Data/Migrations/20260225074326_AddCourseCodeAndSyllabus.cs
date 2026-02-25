using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCodeAndSyllabus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseCode",
                table: "Courses",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SyllabusUrl",
                table: "Courses",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseCode",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SyllabusUrl",
                table: "Courses");
        }
    }
}
