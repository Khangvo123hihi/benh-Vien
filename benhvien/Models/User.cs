using System.ComponentModel.DataAnnotations;

namespace benhvien.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public string? BloodType { get; set; }

        public int? Age { get; set; }

        public string? Address { get; set; }

        public string? Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? HospitalId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("HospitalId")]
        public virtual Hospital? Hospital { get; set; }
    }
}