using Courses.Web.Data;
using Courses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminCoursesController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminCoursesController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _db.Courses
            .Include(c => c.Category)
            .Include(c => c.Teacher)
            .OrderBy(c => c.Title)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdowns(course.CategoryId, course.TeacherId);
        return View(course);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();

        await PopulateDropdowns(course.CategoryId, course.TeacherId);
        return View(course);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdowns(course.CategoryId, course.TeacherId);
        return View(course);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course != null)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(int? selectedCategory = null, int? selectedTeacher = null)
    {
        var categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        var teachers = await _db.Teachers.OrderBy(t => t.Name).ToListAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategory);
        ViewBag.Teachers = new SelectList(teachers, "Id", "Name", selectedTeacher);
    }
}
