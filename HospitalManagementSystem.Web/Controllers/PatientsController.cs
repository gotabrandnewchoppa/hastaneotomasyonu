using HospitalManagementSystem.Web.Data;
using HospitalManagementSystem.Web.Models;
using HospitalManagementSystem.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };

        public PatientsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ─── CRUD ────────────────────────────────────────────────────────────────

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(patients);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,CitizenshipNumber,Phone,Email,Address,DateOfBirth,Gender")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.CreatedAt = DateTime.Now;
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FullName,CitizenshipNumber,Phone,Email,Address,DateOfBirth,Gender,CreatedAt")] Patient patient)
        {
            if (id != patient.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id) =>
            _context.Patients.Any(e => e.Id == id);

        // ─── FILE UPLOAD / DOWNLOAD ──────────────────────────────────────────────

        // GET: Patients/Files/5
        public async Task<IActionResult> Files(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            var reports = await _context.LabReports
                .Where(r => r.PatientId == id)
                .OrderByDescending(r => r.UploadedAt)
                .ToListAsync();

            ViewBag.Patient = patient;
            return View(reports);
        }

        // POST: Patients/Upload/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id, IFormFile file, string? description)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Lütfen bir dosya seçin.";
                return RedirectToAction(nameof(Files), new { id });
            }

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
            {
                TempData["Error"] = "Yalnızca PDF, JPEG, PNG veya DOCX yükleyebilirsiniz.";
                return RedirectToAction(nameof(Files), new { id });
            }

            if (file.Length > 10 * 1024 * 1024) // 10 MB
            {
                TempData["Error"] = "Dosya boyutu 10 MB'ı geçemez.";
                return RedirectToAction(nameof(Files), new { id });
            }

            // Kayıt dizini: wwwroot/uploads/{patientId}/
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", id.ToString());
            Directory.CreateDirectory(uploadDir);

            var storedName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadDir, storedName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            _context.LabReports.Add(new LabReport
            {
                PatientId = id,
                FileName = storedName,
                OriginalName = file.FileName,
                FileSizeBytes = file.Length,
                ContentType = file.ContentType,
                Description = description,
                UploadedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{file.FileName}' başarıyla yüklendi.";
            return RedirectToAction(nameof(Files), new { id });
        }

        // GET: Patients/Download/7
        public async Task<IActionResult> Download(int id)
        {
            var report = await _context.LabReports.FindAsync(id);
            if (report == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads",
                report.PatientId.ToString(), report.FileName);

            if (!System.IO.File.Exists(filePath)) return NotFound();

            return PhysicalFile(filePath, report.ContentType, report.OriginalName);
        }

        // POST: Patients/DeleteFile/7
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFileConfirmed(int id)
        {
            var report = await _context.LabReports.FindAsync(id);
            if (report == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "uploads",
                report.PatientId.ToString(), report.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            var patientId = report.PatientId;
            _context.LabReports.Remove(report);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Dosya silindi.";
            return RedirectToAction(nameof(Files), new { id = patientId });
        }

        // ─── PDF EXPORT ──────────────────────────────────────────────────────────

        // GET: Patients/ExportPdf/5
        public async Task<IActionResult> ExportPdf(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == id)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            var labs = await _context.LabReports
                .Where(l => l.PatientId == id)
                .OrderByDescending(l => l.UploadedAt)
                .ToListAsync();

            var pdfBytes = PatientPdfService.Generate(patient, appointments, labs);
            return File(pdfBytes, "application/pdf", $"hasta_{patient.Id}_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
