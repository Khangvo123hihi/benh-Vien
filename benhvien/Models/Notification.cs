using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace benhvien.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        // 🟢 Bổ sung thuộc tính HospitalId để khớp với SQL của ní
        public int? HospitalId { get; set; }

        // 🟢 Bổ sung thuộc tính TargetBloodTypeId để lọc theo nhóm máu
        public int? TargetBloodTypeId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        public string? Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- CÁC MỐI QUAN HỆ LIÊN KẾT (NAVIGATION PROPERTIES) ---

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("TargetBloodTypeId")]
        public virtual BloodType? TargetBloodType { get; set; }
    }
}