using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Core.Options;
using VeteranAnalyticsSystem.Data;
using VeteranAnalyticsSystem.Models;
using VeteranAnalyticsSystem.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Configure EF Core with your DB context
builder.Services.AddDbContext<GratitudeAmericaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configure RagicImporterService
builder.Services.AddScoped<RagicImporterService>();

// Configure Identity and Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<GratitudeAmericaDbContext>();

// ✅ Keep global policy, but exclude Identity UI
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy)); // This ensures authentication for all controllers except the Identity ones
});

// ✅ Allow anonymous access to Identity UI folder
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/AccessDenied");
    options.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");
});

// Configure redirect paths for login/access denied (optional but recommended)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Register email sender service and options
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

// Build the app
var app = builder.Build();

// Seed Roles and Admin User on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRoles(services);
    await DbInitializer.SeedAdminUser(services);
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Required before UseAuthorization
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route for "Veterans"
app.MapControllerRoute(
    name: "veterans",
    pattern: "Veterans/{action=Index}/{id?}",
    defaults: new { controller = "Veterans", action = "Index" });

// Route for "Veterans Details"
app.MapControllerRoute(
    name: "veteranDetails",
    pattern: "Veterans/Details/{id}", // Route to pass the id for veteran details
    defaults: new { controller = "Veterans", action = "Details" });

// Route for "Events"
app.MapControllerRoute(
    name: "events",
    pattern: "Events/{action=Index}/{id?}",
    defaults: new { controller = "Events", action = "Index" });

// Route for Surveys
app.MapControllerRoute(
    name: "surveys",
    pattern: "Surveys/{action=Index}/{id?}",
    defaults: new { controller = "Surveys", action = "Index" });

// Route for Data Import Page
app.MapControllerRoute(
    name: "data",
    pattern: "Data/{action=Index}/{id?}",
    defaults: new { controller = "Data", action = "Index" });

app.MapControllerRoute(
    name: "users",
    pattern: "Users/{action=Index}/{id?}",  // Default action will be "Index"
    defaults: new { controller = "Users", action = "Index" });


// Route for user settings (specific to user profile settings)
app.MapControllerRoute(
    name: "userSettings",
    pattern: "Users/Settings", // This route will be used for the user settings page
    defaults: new { controller = "Users", action = "Settings" });


// Razor Pages (including Identity UI pages)
app.MapRazorPages();

app.Run();
