using HospitalManagementSystem.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Services
{
    /// <summary>
    /// Finds common available 30-minute appointment slots between a doctor and a patient.
    ///
    /// ALGORITHM: Set Intersection
    /// ─────────────────────────────────────────────────────────────────────────
    ///  1. Generate the full set of slots for the work day   → allSlots
    ///  2. Fetch doctor's booked times from DB               → doctorBooked
    ///  3. Fetch patient's booked times from DB              → patientBooked
    ///  4. Build the union of occupied sets                  → occupied = doctorBooked ∪ patientBooked
    ///  5. Return free slots = allSlots - occupied           → allSlots ∩ complement(occupied)
    /// ─────────────────────────────────────────────────────────────────────────
    /// </summary>
    public class AppointmentSlotService
    {
        private readonly ApplicationDbContext _context;
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan WorkStart = TimeSpan.FromHours(9);
        private static readonly TimeSpan WorkEnd = TimeSpan.FromHours(17);

        public AppointmentSlotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetAvailableSlotsAsync(int doctorId, int patientId, DateOnly date)
        {
            // Step 1 – Generate all slots for the given day
            var allSlots = new HashSet<TimeSpan>();
            for (var t = WorkStart; t + SlotDuration <= WorkEnd; t += SlotDuration)
                allSlots.Add(t);

            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var dayEnd   = dayStart.AddDays(1);

            // Step 2 – Doctor's already‑booked slots
            var doctorBooked = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate >= dayStart
                         && a.AppointmentDate < dayEnd
                         && a.Status != "İptal")
                .Select(a => a.AppointmentDate.TimeOfDay)
                .ToHashSetAsync();

            // Step 3 – Patient's already‑booked slots
            var patientBooked = await _context.Appointments
                .Where(a => a.PatientId == patientId
                         && a.AppointmentDate >= dayStart
                         && a.AppointmentDate < dayEnd
                         && a.Status != "İptal")
                .Select(a => a.AppointmentDate.TimeOfDay)
                .ToHashSetAsync();

            // Step 4 – Union of all occupied slots
            var occupied = new HashSet<TimeSpan>(doctorBooked);
            occupied.UnionWith(patientBooked);

            // Step 5 – Set difference: free = allSlots \ occupied
            var freeSlots = allSlots
                .Except(occupied)
                .OrderBy(t => t)
                .Select(t => $"{t.Hours:D2}:{t.Minutes:D2}")
                .ToList();

            return freeSlots;
        }
    }
}
