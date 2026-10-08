using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicalView.Data;

// Runs once every time the app starts (called from Program.cs).
public static class SeedData
{
    public const string DemoEmail = "demo@clinic.test";
    public const string DemoPassword = "Demo@12345";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // 1. Create the database file and tables if they don't exist yet,
        //    or apply any new migrations. Same thing as running "Update-Database".
        await db.Database.MigrateAsync();

        // 2. Create a demo login so a reviewer can sign in without a Google account.
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        if (await userManager.FindByEmailAsync(DemoEmail) is null)
        {
            var demoUser = new IdentityUser
            {
                UserName = DemoEmail,
                Email = DemoEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(demoUser, DemoPassword);
        }

        // 3. Add the sample patients, but only if the Patient table is empty.
        if (!await db.Patients.AnyAsync())
        {
            db.Patients.AddRange(SampleData.CreatePatients());
            await db.SaveChangesAsync();
        }
    }
}