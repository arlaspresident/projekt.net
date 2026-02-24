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

                if (courses != null)
                {
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

                        //se till att det finns minst en lärare
                        var teacher = context.Teachers.FirstOrDefault();
                        if (teacher == null)
                        {
                            teacher = new Teacher { Name = "Ej angiven" };
                            context.Teachers.Add(teacher);
                            await context.SaveChangesAsync();
                        }

                        context.Courses.Add(new Course
                        {
                            Title = dto.courseName,
                            Credits = dto.points,
                            CategoryId = category.Id,
                            TeacherId = teacher.Id
                        });
                    }

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}