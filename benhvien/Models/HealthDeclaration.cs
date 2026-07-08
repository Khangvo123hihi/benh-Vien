namespace benhvien.Models
{
    public class HealthDeclaration
    {
            public int Id { get; set; }

            public int UserId { get; set; }

            public decimal? Weight { get; set; }

            public decimal? Height { get; set; }

            public decimal? Temperature { get; set; }

            public string? BloodPressure { get; set; }

            public DateTime? LastDonationDate { get; set; }

            public bool? IsSick { get; set; }

            public bool? IsTakingMedicine { get; set; }

            public bool? HasInfectiousRisk { get; set; }

            public bool? DrinkAlcohol24h { get; set; }

            public bool? HasTattooRecently { get; set; }

            public string? MedicalHistory { get; set; }

            public string? Note { get; set; }

            public DateTime CreatedAt { get; set; }

            public User? User { get; set; }
        }
    }
