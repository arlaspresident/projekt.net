using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class Teacher
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    //En lärare kan ha många kurser
    public List<Course> Courses { get; set; } = new();
}