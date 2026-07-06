using System.ComponentModel.DataAnnotations;

namespace benhvien.Models
{
    public class BloodTypeStatistic
    {
        [Key]
        public int BloodTypeId { get; set; }
        public string ABO { get; set; }
        public string Rh { get; set; }
        public string? Description { get; set; }
        public int TotalDonors { get; set; } // Cột số lượng người đăng ký
    }
}