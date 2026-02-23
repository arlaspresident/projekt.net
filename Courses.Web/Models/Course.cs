using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public int Credits { get; set; }

    //FK
    public int CategoryId { get; set; }
    public int TeacherId { get; set; }


    public Category? Category { get; set; }
    public Teacher? Teacher { get; set; }

    public List<Enrollment> Enrollments { get; set; } = new();
}