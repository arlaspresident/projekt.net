using Courses.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminEnrollmentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminEnrollmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? q, int? courseId, int? categoryId)
    {
        //dropdowns
        var courses = await _db.Courses.OrderBy(c => c.Title).ToListAsync();
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Courses = new SelectList(courses, "Id", "Title", courseId);
        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        ViewBag.Query = q;

        //hämta enrollments med relaterad data
        var query = _db.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c!.Category)
            .AsQueryable();

        if (courseId.HasValue)
            query = query.Where(e => e.CourseId == courseId.Value);

        if (categoryId.HasValue)
            query = query.Where(e => e.Course!.CategoryId == categoryId.Value);

        var enrollments = await query
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();

        //hämta studentmejlen från identity
        var userIds = enrollments.Select(e => e.UserId).Distinct().ToList();
        var users = await _db.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? u.UserName ?? u.Id);

        //filtrera på studentmejl om sökning
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            var matchingUserIds = users
                .Where(kv => kv.Value.ToLower().Contains(term))
                .Select(kv => kv.Key)
                .ToHashSet();

            enrollments = enrollments
                .Where(e => matchingUserIds.Contains(e.UserId)
                    || e.Course!.Title.ToLower().Contains(term))
                .ToList();
        }

        ViewBag.Users = users;
        return View(enrollments);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _db.Enrollments.FindAsync(id);
        if (enrollment != null)
        {
            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
