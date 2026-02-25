using Courses.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Courses.Web.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    //offentlig lista över alla kurser
    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        //dropdown
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        ViewBag.Query = q;

        //query
        var query = _context.Courses
            .Include(c => c.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(c => c.Title.Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        //sortera lite så det känns stabilt
        var courses = await query
            .OrderBy(c => c.Title)
            .Take(200) //så den inte renderar 4k rader
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Details(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isEnrolled = userId != null && await _context.Enrollments
            .AnyAsync(e => e.CourseId == id && e.UserId == userId);

        ViewBag.IsEnrolled = isEnrolled;

        return View(course);
    }

    public async Task<IActionResult> Count()
    {
        var count = await _context.Courses.CountAsync();
        return Content($"Antal kurser i DB: {count}");
    }
}