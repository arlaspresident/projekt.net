using System.ComponentModel.DataAnnotations;

namespace Courses.Web.Models;

public class DocumentRequest
{
    public int Id { get; set; }

    [Required]
    public string StudentUserId { get; set; } = string.Empty;

    [Required]
    public string AdminUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    public bool IsFulfilled { get; set; } = false;

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? FulfilledAt { get; set; }
}
