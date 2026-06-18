using benhvien.Models;
using Microsoft.EntityFrameworkCore;

namespace benhvien.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<BloodRequest> BloodRequests { get; set; }

        public DbSet<DonationAppointment> DonationAppointments { get; set; }

        public DbSet<DonationHistory> DonationHistories { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }

        public DbSet<Article> Articles { get; set; }
    }
}