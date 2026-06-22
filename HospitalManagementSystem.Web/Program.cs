using HospitalManagementSystem.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache(); // For simple local testing before Redis is fully setup.

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

// Türkçe URL desteği
app.MapControllerRoute(
    name: "hastalar",
    pattern: "Hastalar/{action=Index}/{id?}",
    defaults: new { controller = "Patients" });

app.MapControllerRoute(
    name: "doktorlar",
    pattern: "Doktorlar/{action=Index}/{id?}",
    defaults: new { controller = "Doctors" });

app.MapControllerRoute(
    name: "randevular",
    pattern: "Randevular/{action=Index}/{id?}",
    defaults: new { controller = "Appointments" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
