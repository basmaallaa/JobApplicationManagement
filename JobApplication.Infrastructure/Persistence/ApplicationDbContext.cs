using JobApplication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace JobApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<JobCandidateApplication> JobCandidateApplications { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing jobs have no owner, so RecruiterId is nullable.
            modelBuilder.Entity<Job>()
                .HasOne<Recruiter>()
                .WithMany()
                .HasForeignKey(j => j.RecruiterId);

            modelBuilder.Entity<Job>()
                .Property(j => j.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
