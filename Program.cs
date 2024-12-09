using Microsoft.EntityFrameworkCore;
using VeteranAnalyticsSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<GratitudeAmericaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Additional route for VeteransController
app.MapControllerRoute(
    name: "veterans",
    pattern: "Veterans/{action=Index}/{id?}",
    defaults: new { controller = "Veterans" });

// Additional route for EventsController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Additional route for VolunteersController
app.MapControllerRoute(
    name: "volunteers",
    pattern: "Volunteers/{action=Index}/{id?}",
    defaults: new { controller = "Volunteers", action = "Index" });

app.Run();
