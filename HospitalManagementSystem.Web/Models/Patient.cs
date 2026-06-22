using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Web.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hasta Adı zorunludur.")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        public string CitizenshipNumber { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        public string Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        // İlişkiler
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
