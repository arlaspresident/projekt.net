using Courses.Web.Data;
using Courses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Student")]
public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public EnrollmentsController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int courseId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        // säkerställ att kursen finns
        var courseExists = await _db.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
            return NotFound();

        // dublettskydd (även om DB har unique index)
        var alreadyEnrolled = await _db.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (!alreadyEnrolled)
        {
            _db.Enrollments.Add(new Enrollment
            {
                CourseId = courseId,
                UserId = userId
            });

            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Courses", new { id = courseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int courseId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (enrollment != null)
        {
            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Courses", new { id = courseId });
    }

    [HttpGet]
    public async Task<IActionResult> MyCourses()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId))
            return Challenge();

        var courses = await _db.Enrollments
            .Where(e => e.UserId == userId)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrolledAt)
            .Select(e => e.Course!)
            .ToListAsync();

        return View(courses);
    }
}