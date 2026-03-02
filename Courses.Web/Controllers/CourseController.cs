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
    public async Task<IActionResult> Index(string? q, int? categoryId, int page = 1, string sort = "titel")
    {
        const int pageSize = 25;

        //dropdown
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        ViewBag.Query = q;
        ViewBag.Sort = sort;

        //query
        var query = _context.Courses
            .Include(c => c.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == categoryId.Value);
        }

        //sortering
        query = sort switch
        {
            "hp_asc"     => query.OrderBy(c => c.Credits),
            "hp_desc"    => query.OrderByDescending(c => c.Credits),
            "titel_desc" => query.OrderByDescending(c => c.Title),
            _            => query.OrderBy(c => c.Title)
        };

        //paginering
        var totalCount = await query.CountAsync();
        var courses = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return PartialView("_CourseList", courses);

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