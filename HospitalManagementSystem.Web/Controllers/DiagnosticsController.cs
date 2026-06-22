using HospitalManagementSystem.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Web.Controllers
{
    /// <summary>
    /// Exposes diagnostic endpoints for live monitoring of background services.
    /// </summary>
    public class DiagnosticsController : Controller
    {
        // GET: /Diagnostics/PidStatus  → returns PID log as JSON (for realtime demo)
        public IActionResult PidStatus()
        {
            lock (FluidPumpBackgroundService.RecentLogs)
            {
                var logs = FluidPumpBackgroundService.RecentLogs
                    .OrderByDescending(l => l.Timestamp)
                    .ToList();
                return Json(logs);
            }
        }

        // GET: /Diagnostics/PidMonitor → renders live dashboard view
        public IActionResult PidMonitor()
        {
            return View();
        }
    }
}
