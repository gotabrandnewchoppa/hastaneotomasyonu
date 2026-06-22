using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Web.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        public string? RoomNumber { get; set; }

        [Required]
        public string Phone { get; set; }

        public string? Email { get; set; }

        // İlişkiler
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
