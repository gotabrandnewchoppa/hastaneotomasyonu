using HospitalManagementSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<LabReport> LabReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Query Filters (Soft Delete)
            modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Doctor>().HasQueryFilter(d => !d.IsDeleted);
            modelBuilder.Entity<Appointment>().HasQueryFilter(a => !a.IsDeleted);

            // İlişkiler
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<LabReport>()
                .HasOne(l => l.Patient)
                .WithMany()
                .HasForeignKey(l => l.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Initial Seed Data (10 Hastalar, 5 Doktorlar)
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FullName = "Dr. Mehmet Öz", Specialization = "Kardiyoloji", Phone = "05551112233", RoomNumber = "K-02" },
                new Doctor { Id = 2, FullName = "Op. Dr. Ayşe Yılmaz", Specialization = "Genel Cerrahi", Phone = "05552223344", RoomNumber = "G-05" },
                new Doctor { Id = 3, FullName = "Uzm. Dr. Kemal Sunal", Specialization = "Dahiliye", Phone = "05553334455", RoomNumber = "D-10" },
                new Doctor { Id = 4, FullName = "Op. Dr. Cüneyt Arkın", Specialization = "Ortopedi", Phone = "05554445566", RoomNumber = "O-01" },
                new Doctor { Id = 5, FullName = "Dr. Fatma Girik", Specialization = "Çocuk Hastalıkları", Phone = "05555556677", RoomNumber = "C-08" }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FullName = "Ahmet Yılmaz", CitizenshipNumber = "12345678901", Phone = "05321112233", Gender = "Erkek", DateOfBirth = new DateTime(1980, 5, 10), CreatedAt = new DateTime(2026, 2, 28) },
                new Patient { Id = 2, FullName = "Fatma Şahin", CitizenshipNumber = "23456789012", Phone = "05322223344", Gender = "Kadın", DateOfBirth = new DateTime(1992, 8, 15), CreatedAt = new DateTime(2026, 3, 1) },
                new Patient { Id = 3, FullName = "Ali Koç", CitizenshipNumber = "34567890123", Phone = "05323334455", Gender = "Erkek", DateOfBirth = new DateTime(1975, 2, 20), CreatedAt = new DateTime(2026, 3, 2) },
                new Patient { Id = 4, FullName = "Ayşe Demir", CitizenshipNumber = "45678901234", Phone = "05324445566", Gender = "Kadın", DateOfBirth = new DateTime(1988, 11, 5), CreatedAt = new DateTime(2026, 3, 3) },
                new Patient { Id = 5, FullName = "Mustafa Çelik", CitizenshipNumber = "56789012345", Phone = "05325556677", Gender = "Erkek", DateOfBirth = new DateTime(1960, 1, 30), CreatedAt = new DateTime(2026, 3, 4) },
                new Patient { Id = 6, FullName = "Zeynep Kaya", CitizenshipNumber = "67890123456", Phone = "05326667788", Gender = "Kadın", DateOfBirth = new DateTime(2000, 4, 18), CreatedAt = new DateTime(2026, 3, 5) },
                new Patient { Id = 7, FullName = "Mehmet Can", CitizenshipNumber = "78901234567", Phone = "05327778899", Gender = "Erkek", DateOfBirth = new DateTime(1995, 7, 22), CreatedAt = new DateTime(2026, 3, 6) },
                new Patient { Id = 8, FullName = "Elif Yıldırım", CitizenshipNumber = "89012345678", Phone = "05328889900", Gender = "Kadın", DateOfBirth = new DateTime(1985, 9, 12), CreatedAt = new DateTime(2026, 3, 7) },
                new Patient { Id = 9, FullName = "Hasan Aydın", CitizenshipNumber = "90123456789", Phone = "05329990011", Gender = "Erkek", DateOfBirth = new DateTime(1970, 3, 8), CreatedAt = new DateTime(2026, 3, 8) },
                new Patient { Id = 10, FullName = "Sibel Ak", CitizenshipNumber = "01234567890", Phone = "05320001122", Gender = "Kadın", DateOfBirth = new DateTime(1990, 12, 25), CreatedAt = new DateTime(2026, 3, 9) }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, PatientId = 1, DoctorId = 1, AppointmentDate = new DateTime(2026, 3, 11, 9, 0, 0), Status = "Onaylandı", Reason = "Kontrol / Muayene" },
                new Appointment { Id = 2, PatientId = 2, DoctorId = 2, AppointmentDate = new DateTime(2026, 3, 11, 14, 0, 0), Status = "Onaylandı", Reason = "Ağrı Şikâyeti" },
                new Appointment { Id = 3, PatientId = 3, DoctorId = 1, AppointmentDate = new DateTime(2026, 3, 12, 10, 0, 0), Status = "İptal", Reason = "Tahlil / Tetkik" }
            );
        }
    }
}
