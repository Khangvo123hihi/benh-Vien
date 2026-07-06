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

        public int? Age { get; set; }

        public string? Address { get; set; }

        public int? RoleId { get; set; }

        public virtual Role? Role { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? HospitalId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("HospitalId")]
        public virtual Hospital? Hospital { get; set; }
        // 🟢 THÊM 2 DÒNG NÀY VÀO FILE User.cs
        public int? BloodTypeId { get; set; } // Khóa ngoại lưu ID nhóm máu (số nguyên)

        public virtual BloodType? BloodType { get; set; } // Liên kết đối tượng bảng BloodType
    }
}