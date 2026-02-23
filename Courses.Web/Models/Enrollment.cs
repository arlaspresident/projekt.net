using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class Enrollment
{
    public int Id { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    //FK
    public string UserId { get; set; } = string.Empty;
    public int CourseId { get; set; }

    public Course? Course { get; set; }
}