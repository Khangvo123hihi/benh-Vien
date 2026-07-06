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
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        public DbSet<DonationAppointment> DonationAppointments { get; set; }

        public DbSet<DonationHistory> DonationHistories { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Role> Roles { get; set; }
        // 🟢 THÊM DÒNG NÀY VÀO FILE ApplicationDbContext.cs
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<BloodTypeStatistic> BloodTypeStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Báo cho EF Core biết đây là SQL View, không phải Table
            modelBuilder.Entity<BloodTypeStatistic>()
                        .ToView("View_BloodTypeStatistics")
                        .HasNoKey(); // Nếu EF Core yêu cầu (hoặc dùng [Key] ở model trên đều được)

        }
    }
}