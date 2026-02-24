using Courses.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    //offentlig lista över alla kurser
    public async Task<IActionResult> Index()
    {
        var courses = await _context.Courses
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Count()
{
    var count = await _context.Courses.CountAsync();
    return Content($"Antal kurser i DB: {count}");
}
}