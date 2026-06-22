using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Web.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        /// <summary>Randevu sebebi / şikâyeti</summary>
        [Required(ErrorMessage = "Randevu sebebi seçilmelidir.")]
        public string Reason { get; set; } = string.Empty;

        public string? Notes { get; set; }

        [Required]
        public string Status { get; set; } = "Bekliyor"; // Onaylandı, Bekliyor, İptal, Tamamlandı

        public bool IsDeleted { get; set; } = false;
    }
}
