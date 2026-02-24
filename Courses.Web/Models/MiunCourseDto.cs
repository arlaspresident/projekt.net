namespace Courses.Web.Models;

public class MiunCourseDto
{
    public string courseCode { get; set; } = string.Empty;
    public string courseName { get; set; } = string.Empty;
    public double points { get; set; }
    public string subject { get; set; } = string.Empty;
    public string syllabus { get; set; } = string.Empty;
}