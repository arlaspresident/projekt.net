using Courses.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Courses.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        //skapa roller
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Student"))
            await roleManager.CreateAsync(new IdentityRole("Student"));

        //skapa admin
        var adminEmail = "admin@courses.se";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        //skapa student
        var studentEmail = "student@courses.se";
        var student = await userManager.FindByEmailAsync(studentEmail);

        if (student == null)
        {
            student = new IdentityUser
            {
                UserName = studentEmail,
                Email = studentEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(student, "Student123!");
            await userManager.AddToRoleAsync(student, "Student");
        }

        //seed lärare
        if (!context.Teachers.Any())
        {
            var teacherNames = new[]
            {
                ("Anna Lindström", "anna.lindstrom@miun.se"),
                ("Erik Johansson", "erik.johansson@miun.se"),
                ("Maria Petersson", "maria.petersson@miun.se"),
                ("Lars Nilsson", "lars.nilsson@miun.se"),
                ("Sofia Bergström", "sofia.bergstrom@miun.se"),
                ("Johan Eriksson", "johan.eriksson@miun.se"),
                ("Karin Andersson", "karin.andersson@miun.se"),
                ("Anders Holm", "anders.holm@miun.se"),
            };

            foreach (var (name, email) in teacherNames)
                context.Teachers.Add(new Teacher { Name = name, Email = email });

            await context.SaveChangesAsync();
        }

        //seed kurser från json
        if (!context.Courses.Any())
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data",
                "SeedData",
                "miun_courses.json"
            );

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var courses = JsonSerializer.Deserialize<List<MiunCourseDto>>(json);
                courses = courses?
    .GroupBy(c => c.courseCode)
    .Select(g => g.First())
    .ToList();

                if (courses != null)
                {
                    var teachers = context.Teachers.ToList();
                    int teacherIndex = 0;

                    foreach (var dto in courses)
                    {
                        //skapa kategori om den inte finns
                        var category = context.Categories
                            .FirstOrDefault(c => c.Name == dto.subject);

                        if (category == null)
                        {
                            category = new Category { Name = dto.subject };
                            context.Categories.Add(category);
                            await context.SaveChangesAsync();
                        }

                        var exists = await context.Courses
     .AnyAsync(c => c.CourseCode == dto.courseCode);

                        if (!exists)
                        {
                            //fördela kurser jämnt bland lärarna
                            var teacher = teachers[teacherIndex % teachers.Count];
                            teacherIndex++;

                            context.Courses.Add(new Course
                            {
                                CourseCode = dto.courseCode,
                                Title = dto.courseName,
                                Credits = dto.points,
                                CategoryId = category.Id,
                                TeacherId = teacher.Id,
                                SyllabusUrl = dto.syllabus
                            });
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}