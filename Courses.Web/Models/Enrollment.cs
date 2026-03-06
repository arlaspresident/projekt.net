using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class Enrollment
{
    public int Id { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public bool IsApproved { get; set; } = false;

    //FK till identity anv
    [Required]
    public string UserId { get; set; } = string.Empty;

    //FK till course
    [Required]
    public int CourseId { get; set; }

    public Course? Course { get; set; }
}