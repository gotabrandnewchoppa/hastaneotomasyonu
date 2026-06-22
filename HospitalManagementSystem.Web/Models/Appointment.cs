using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Web.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public string? Notes { get; set; }

        [Required]
        public string Status { get; set; } = "Onaylandı"; // Onaylandı, İptal, Tamamlandı
    }
}
