using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [NotMapped]
        public int? Age
        {
            get
            {
                if (DateOfBirth == null)
                    return null;

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;

                if (DateOfBirth.Value.Date > today.AddYears(-age))
                    age--;

                return age;
            }
        }
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