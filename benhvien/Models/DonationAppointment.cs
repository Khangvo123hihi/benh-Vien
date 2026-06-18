using System.ComponentModel.DataAnnotations.Schema;

namespace benhvien.Models
{
    public class DonationAppointment
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BloodRequestId { get; set; }

        public DateTime DonateDate { get; set; }

        public string? Location { get; set; }

        public string? Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? HospitalName { get; set; }

        public string? BloodType { get; set; }

        public string? Note { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("BloodRequestId")]
        public virtual BloodRequest? BloodRequest { get; set; }
    }
}