using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = string.Empty;

    //En kategori kan ha många kurser
    public List<Course> Courses { get; set; } = new();
}