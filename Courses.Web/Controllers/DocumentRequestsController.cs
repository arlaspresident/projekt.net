using System.Security.Claims;
using Courses.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Student")]
public class DocumentRequestsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public DocumentRequestsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var requests = await _db.DocumentRequests
            .Where(r => r.StudentUserId == userId)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        return View(requests);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(int id, IFormFile file)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var request = await _db.DocumentRequests
            .FirstOrDefaultAsync(r => r.Id == id && r.StudentUserId == userId);

        if (request == null)
            return NotFound();

        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Välj en PDF fil.";
            return RedirectToAction(nameof(Index));
        }

        if (file.ContentType != "application/pdf")
        {
            TempData["Error"] = "Endast PDF filer tillåts.";
            return RedirectToAction(nameof(Index));
        }

        if (file.Length > 10 * 1024 * 1024)
        {
            TempData["Error"] = "Filen får inte vara större än 10mb.";
            return RedirectToAction(nameof(Index));
        }

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "documents");
        Directory.CreateDirectory(uploadsDir);

        var storedName = $"{Guid.NewGuid()}.pdf";
        var filePath = Path.Combine(uploadsDir, storedName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        //ta bort gammal fil om de finns en
        if (request.FilePath != null && System.IO.File.Exists(request.FilePath))
            System.IO.File.Delete(request.FilePath);

        request.FileName = file.FileName;
        request.FilePath = filePath;
        request.IsFulfilled = true;
        request.FulfilledAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Filen har laddats upp.";
        return RedirectToAction(nameof(Index));
    }
}
