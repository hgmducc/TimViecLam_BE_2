using Microsoft.EntityFrameworkCore;
// using System.ComponentModel; // Bạn không cần 'using' này
using TimViecLam.Models.Domain; // Thay thế bằng namespace của bạn

namespace TimViecLam.Data // Thay thế bằng namespace Data của bạn
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<Administrator> Administrators { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình Bảng Users ---
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // --- Cấu hình Quan hệ 1-1 (Shared Primary Key) ---

            // 1. User <-> Employer
            modelBuilder.Entity<User>()
                .HasOne(user => user.Employer)
                .WithOne(employer => employer.User)
                .HasForeignKey<Employer>(employer => employer.EmployerID);

            // 2. User <-> Candidate
            modelBuilder.Entity<User>()
                .HasOne(user => user.Candidate)
                .WithOne(candidate => candidate.User)
                .HasForeignKey<Candidate>(candidate => candidate.CandidateID);

            // 3. User <-> Administrator
            modelBuilder.Entity<User>()
                .HasOne(user => user.Administrator)
                .WithOne(admin => admin.User)
                .HasForeignKey<Administrator>(admin => admin.AdminID);
        }
    }
}