namespace benhvien.Models
{
    public class DonationHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? HospitalName { get; set; }

        public string? BloodType { get; set; }

        public DateTime DonateDate { get; set; }

        public string? Note { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}