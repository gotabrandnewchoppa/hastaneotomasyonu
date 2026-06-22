using HospitalManagementSystem.Web.Data;
using HospitalManagementSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Randevu sebepleri listesi
        public static readonly List<string> ReasonList = new()
        {
            "Kontrol / Muayene",
            "Ağrı Şikâyeti",
            "Ameliyat Sonrası Takip",
            "Tahlil / Tetkik",
            "Aşı / İlaç Uygulaması",
            "Psikolojik Destek",
            "Diş Şikâyeti",
            "Göz Muayenesi",
            "Deri Hastalığı",
            "Çocuk Hastalığı",
            "Kronik Hastalık Takibi",
            "Acil Başvuru",
            "Diğer"
        };

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
            return View(appointments);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewBag.Patients = new SelectList(_context.Patients.OrderBy(p => p.FullName), "Id", "FullName");
            ViewBag.Doctors = new SelectList(_context.Doctors.OrderBy(d => d.FullName), "Id", "FullName");
            ViewBag.Reasons = new SelectList(ReasonList);
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DoctorId,AppointmentDate,Reason,Notes,Status")] Appointment appointment)
        {
            // Navigation property'ler formdan gönderilmez, ModelState'ten temizle
            ModelState.Remove(nameof(Appointment.Patient));
            ModelState.Remove(nameof(Appointment.Doctor));

            // Yıl aralığı kontrolü
            if (appointment.AppointmentDate.Year < 2000 || appointment.AppointmentDate.Year > 2099)
            {
                ModelState.AddModelError("AppointmentDate", "Tarih 2000 ile 2099 yılları arasında olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = new SelectList(_context.Patients.OrderBy(p => p.FullName), "Id", "FullName", appointment.PatientId);
            ViewBag.Doctors = new SelectList(_context.Doctors.OrderBy(d => d.FullName), "Id", "FullName", appointment.DoctorId);
            ViewBag.Reasons = new SelectList(ReasonList, appointment.Reason);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            ViewBag.Patients = new SelectList(_context.Patients.OrderBy(p => p.FullName), "Id", "FullName", appointment.PatientId);
            ViewBag.Doctors = new SelectList(_context.Doctors.OrderBy(d => d.FullName), "Id", "FullName", appointment.DoctorId);
            ViewBag.Reasons = new SelectList(ReasonList, appointment.Reason);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,AppointmentDate,Reason,Notes,Status")] Appointment appointment)
        {
            if (id != appointment.Id) return NotFound();

            // Navigation property'ler formdan gönderilmez, ModelState'ten temizle
            ModelState.Remove(nameof(Appointment.Patient));
            ModelState.Remove(nameof(Appointment.Doctor));

            // Yıl aralığı kontrolü
            if (appointment.AppointmentDate.Year < 2000 || appointment.AppointmentDate.Year > 2099)
            {
                ModelState.AddModelError("AppointmentDate", "Tarih 2000 ile 2099 yılları arasında olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Appointments.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Randevu başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = new SelectList(_context.Patients.OrderBy(p => p.FullName), "Id", "FullName", appointment.PatientId);
            ViewBag.Doctors = new SelectList(_context.Doctors.OrderBy(d => d.FullName), "Id", "FullName", appointment.DoctorId);
            ViewBag.Reasons = new SelectList(ReasonList, appointment.Reason);
            return View(appointment);
        }

        // POST: Appointments/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Tamamlandı";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu tamamlandı olarak işaretlendi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.IsDeleted = true;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/ExportListPdf
        public async Task<IActionResult> ExportListPdf()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
                
            var pdfBytes = HospitalManagementSystem.Web.Services.ListPdfService.ExportAppointments(appointments);
            return File(pdfBytes, "application/pdf", $"Randevular_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
