using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using HospitalManagementSystem.Web.Data;
using HospitalManagementSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var culture = new CultureInfo("tr-TR");

        // ── İstatistik Kartları ──────────────────────────────────────────────
        ViewBag.TotalPatients       = await _context.Patients.CountAsync();
        ViewBag.TotalDoctors        = await _context.Doctors.CountAsync();
        ViewBag.TodayAppointments   = await _context.Appointments
                                            .CountAsync(a => a.AppointmentDate.Date == today);
        ViewBag.PendingAppointments = await _context.Appointments
                                            .CountAsync(a => a.Status == "Bekliyor");

        // ── Son 7 Günün Randevu Grafiği (Chart.js) ──────────────────────────
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-6 + i))
            .ToList();

        var chartCounts = new List<int>();
        foreach (var day in last7Days)
        {
            var count = await _context.Appointments
                .CountAsync(a => a.AppointmentDate.Date == day);
            chartCounts.Add(count);
        }

        ViewBag.ChartLabels = JsonSerializer.Serialize(
            last7Days.Select(d => d.ToString("dd MMM", culture)).ToList());
        ViewBag.ChartData = JsonSerializer.Serialize(chartCounts);

        // ── Son 5 Randevu ────────────────────────────────────────────────────
        var recentAppointments = await _context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .Take(5)
            .ToListAsync();
        ViewBag.RecentAppointments = recentAppointments;

        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
