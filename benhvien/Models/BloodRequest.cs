namespace benhvien.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        public int HospitalId { get; set; }

        public User? Hospital { get; set; }

        public string? BloodType { get; set; }

        public int QuantityNeeded { get; set; }

        public string? Description { get; set; }

        public bool IsUrgent { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}