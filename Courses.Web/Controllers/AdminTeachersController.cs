using Courses.Web.Data;
using Courses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminTeachersController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminTeachersController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var teachers = await _db.Teachers.OrderBy(t => t.Name).ToListAsync();
        return View(teachers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Teacher teacher)
    {
        if (ModelState.IsValid)
        {
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(teacher);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _db.Teachers.FindAsync(id);
        if (teacher == null) return NotFound();

        return View(teacher);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Teacher teacher)
    {
        if (id != teacher.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _db.Teachers.Update(teacher);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(teacher);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _db.Teachers.FindAsync(id);
        if (teacher != null)
        {
            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
