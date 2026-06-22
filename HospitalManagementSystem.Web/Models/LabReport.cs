using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Web.Models
{
    public class LabReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        [Required]
        public string FileName { get; set; } = string.Empty;        // depolanan ad (GUID)

        [Required]
        public string OriginalName { get; set; } = string.Empty;    // kullanıcının orijinal dosya adı

        public long FileSizeBytes { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public string? Description { get; set; }
    }
}
