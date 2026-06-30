using System.ComponentModel.DataAnnotations.Schema;

namespace benhvien.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        public int HospitalId { get; set; }
        
        [ForeignKey(nameof(HospitalId))]
        public virtual Hospital? Hospital { get; set; }

        public string? BloodType { get; set; }

        public int QuantityNeeded { get; set; }

        public string? Description { get; set; }

        public bool IsUrgent { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}