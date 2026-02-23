using Courses.Web.Models;
using Microsoft.AspNetCore.Identity;

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

        //seed kategorier
        if (!context.Categories.Any())
        {
            var web = new Category { Name = "Webbutveckling" };
            var js = new Category { Name = "JavaScript" };
            var cs = new Category { Name = "C#" };

            context.Categories.AddRange(web, js, cs);
            await context.SaveChangesAsync();

            //seed lärare
            var t1 = new Teacher { Name = "Anna Svensson", Email = "anna@courses.se" };
            var t2 = new Teacher { Name = "Erik Larsson", Email = "erik@courses.se" };
            var t3 = new Teacher { Name = "Maria Nilsson", Email = "maria@courses.se" };

            context.Teachers.AddRange(t1, t2, t3);
            await context.SaveChangesAsync();

            //seed kurser
            context.Courses.AddRange(
                new Course { Title = "ASP.NET Core MVC", Credits = 7, CategoryId = web.Id, TeacherId = t1.Id },
                new Course { Title = "Blazor Basics", Credits = 5, CategoryId = web.Id, TeacherId = t2.Id },
                new Course { Title = "Modern JavaScript", Credits = 7, CategoryId = js.Id, TeacherId = t3.Id },
                new Course { Title = "TypeScript Advanced", Credits = 5, CategoryId = js.Id, TeacherId = t2.Id },
                new Course { Title = "C# Fundamentals", Credits = 7, CategoryId = cs.Id, TeacherId = t1.Id },
                new Course { Title = "LINQ & EF Core", Credits = 5, CategoryId = cs.Id, TeacherId = t3.Id }
            );

            await context.SaveChangesAsync();
        }
    }
}