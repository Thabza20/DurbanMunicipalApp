using DurbanMunicipalApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // set to true if you want email confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed the Admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.UserProfiles.Any(u => u.Email == "thabiso.ntini@icloud.com"))
    {
        var adminUser = new DurbanMunicipalApp.Models.UserProfile
        {
            Email = "thabiso.ntini@icloud.com",
            Password = "$2a$12$Af1kwTrCla.U3svlGyBDxuxwV2.xR6VOmGPL2WwGnQKPM0f6gec8C", // hashed
            IsActive = true,
            UserType = "Admin"
        };

        db.UserProfiles.Add(adminUser);
        db.SaveChanges();

        db.Admins.Add(new DurbanMunicipalApp.Models.Admin
        {
            UserId = adminUser.UserId,
            AdminName = "Thabiso Ntini"
        });
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication(); // must add authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
