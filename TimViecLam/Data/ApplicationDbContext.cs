using Microsoft.EntityFrameworkCore;
using TimViecLam.Models.Domain;

namespace TimViecLam.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Core tables
        public DbSet<User> Users { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Administrator> Administrators { get; set; }

        // New tables
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // USER CONFIGURATION
            // ==========================================

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // Default values
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasDefaultValue("Active");

            // ==========================================
            // ONE-TO-ONE RELATIONSHIPS (Shared Primary Key)
            // ==========================================

            // User <-> Candidate (1:0.. 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Candidate)
                .WithOne(c => c.User)
                .HasForeignKey<Candidate>(c => c.CandidateID)
                .OnDelete(DeleteBehavior.Cascade);

            // User <-> Employer (1:0..1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employer)
                .WithOne(e => e.User)
                .HasForeignKey<Employer>(e => e.EmployerID)
                .OnDelete(DeleteBehavior.Cascade);

            // User <-> Administrator (1:0.. 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Administrator)
                .WithOne(a => a.User)
                .HasForeignKey<Administrator>(a => a.AdminID)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // CANDIDATE CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Candidate>()
                .Property(c => c.ProfileCompleteness)
                .HasDefaultValue(0);

            modelBuilder.Entity<Candidate>()
                .Property(c => c.LastUpdated)
                .HasDefaultValueSql("GETUTCDATE()");

            // ==========================================
            // CANDIDATE ONE-TO-MANY RELATIONSHIPS
            // ==========================================

            // Candidate <-> Educations (1:N)
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Educations)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateID)
                .OnDelete(DeleteBehavior.Cascade);

            // Candidate <-> Experiences (1:N)
            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Experiences)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateID)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // EDUCATION CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Education>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // ==========================================
            // EXPERIENCE CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Experience>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Experience>()
                .Property(e => e.IsCurrent)
                .HasDefaultValue(false);

            // ==========================================
            // EMPLOYER CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Employer>()
                .Property(e => e.VerificationStatus)
                .HasDefaultValue("Pending");

            modelBuilder.Entity<Employer>()
                .Property(e => e.LastUpdated)
                .HasDefaultValueSql("GETUTCDATE()");

            // ==========================================
            // EMPLOYER ONE-TO-MANY RELATIONSHIPS
            // ==========================================

            // Employer <-> JobPostings (1:N)
            modelBuilder.Entity<Employer>()
                .HasMany(e => e.JobPostings)
                .WithOne(j => j.Employer)
                .HasForeignKey(j => j.EmployerID)
                .OnDelete(DeleteBehavior.Cascade);

            // ==========================================
            // JOB POSTING CONFIGURATION
            // ==========================================

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.Status)
                .HasDefaultValue("Draft");

            // ✅ XÓA: DisplayStatus không còn tồn tại
            // modelBuilder.Entity<JobPosting>()
            //     .Property(j => j.DisplayStatus)
            //     .HasDefaultValue("Active");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.ViewCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.ApplicationCount)
                .HasDefaultValue(0);

            modelBuilder.Entity<JobPosting>()
                .Property(j => j.VacancyCount)
                .HasDefaultValue(1);

            // Index for searching
            modelBuilder.Entity<JobPosting>()
                .HasIndex(j => j.Status);

            modelBuilder.Entity<JobPosting>()
                .HasIndex(j => j.JobTitle);

            // ✅ XÓA: Index DisplayStatus
            // modelBuilder.Entity<JobPosting>()
            //     .HasIndex(j => j.DisplayStatus);

            // ==========================================
            // JOB APPLICATION CONFIGURATION
            // ==========================================

            modelBuilder.Entity<JobApplication>()
                .Property(ja => ja.Status)
                .HasDefaultValue("Submitted");

            modelBuilder.Entity<JobApplication>()
                .Property(ja => ja.AppliedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // JobPosting <-> JobApplications (1:N) - Cascade (CHỦ YẾU)
            modelBuilder.Entity<JobPosting>()
                .HasMany(j => j.JobApplications)
                .WithOne(ja => ja.JobPosting)
                .HasForeignKey(ja => ja.JobPostingID)
                .OnDelete(DeleteBehavior.Cascade);

            // Candidate <-> JobApplications (1:N) - NO ACTION (PHỤ)
            // ✅ ĐÚNG: Để NoAction tránh multiple cascade paths
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Candidate)
                .WithMany()
                .HasForeignKey(ja => ja.CandidateID)
                .OnDelete(DeleteBehavior.NoAction);

            // Unique constraint: Candidate chỉ ứng tuyển 1 lần cho 1 job
            modelBuilder.Entity<JobApplication>()
                .HasIndex(ja => new { ja.CandidateID, ja.JobPostingID })
                .IsUnique();

            // ==========================================
            // SAVED JOB CONFIGURATION
            // ==========================================

            modelBuilder.Entity<SavedJob>()
                .Property(sj => sj.SavedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Candidate <-> SavedJobs (1:N) - Cascade
            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.Candidate)
                .WithMany()
                .HasForeignKey(sj => sj.CandidateID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ SỬA: JobPosting <-> SavedJobs: NoAction → Cascade
            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.JobPosting)
                .WithMany()
                .HasForeignKey(sj => sj.JobPostingID)
                .OnDelete(DeleteBehavior.NoAction);  // ← ĐÚNG: NoAction

            // Unique constraint: Candidate chỉ lưu 1 lần cho 1 job
            modelBuilder.Entity<SavedJob>()
                .HasIndex(sj => new { sj.CandidateID, sj.JobPostingID })
                .IsUnique();

            // ==========================================
            // NOTIFICATION CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasDefaultValue("Info");

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // User <-> Notifications (1:N)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for querying unread notifications
            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserID, n.IsRead });

            // ==========================================
            // ADMINISTRATOR CONFIGURATION
            // ==========================================

            modelBuilder.Entity<Administrator>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}