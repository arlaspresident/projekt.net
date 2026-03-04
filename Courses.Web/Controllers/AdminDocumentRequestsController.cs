using System.Security.Claims;
using Courses.Web.Data;
using Courses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminDocumentRequestsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminDocumentRequestsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _db.DocumentRequests
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        var userIds = requests
            .SelectMany(r => new[] { r.StudentUserId, r.AdminUserId })
            .Distinct()
            .ToList();

        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? u.UserName ?? u.Id);

        ViewBag.Users = users;
        return View(requests);
    }

    public async Task<IActionResult> Create()
    {
        var students = await _db.Users
            .Where(u => u.Email != null && u.Email.EndsWith("@student.courses.se"))
            .OrderBy(u => u.Email)
            .ToListAsync();

        ViewBag.Students = new SelectList(students, "Id", "Email");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string studentUserId, string message)
    {
        if (string.IsNullOrWhiteSpace(studentUserId) || string.IsNullOrWhiteSpace(message))
        {
            ModelState.AddModelError("", "Student och meddelande krävs.");
            var students = await _db.Users
                .Where(u => u.Email != null && u.Email.EndsWith("@student.courses.se"))
                .OrderBy(u => u.Email)
                .ToListAsync();
            ViewBag.Students = new SelectList(students, "Id", "Email");
            return View();
        }

        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var request = new DocumentRequest
        {
            StudentUserId = studentUserId,
            AdminUserId = adminId,
            Message = message
        };

        _db.DocumentRequests.Add(request);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var request = await _db.DocumentRequests.FindAsync(id);
        if (request != null)
        {
            if (request.FilePath != null && System.IO.File.Exists(request.FilePath))
                System.IO.File.Delete(request.FilePath);

            _db.DocumentRequests.Remove(request);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
