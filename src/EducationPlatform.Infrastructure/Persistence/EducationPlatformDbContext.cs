using EducationPlatform.Domain.FirstSlice;
using Microsoft.EntityFrameworkCore;

namespace EducationPlatform.Infrastructure.Persistence;

public sealed class EducationPlatformDbContext(DbContextOptions<EducationPlatformDbContext> options)
    : DbContext(options)
{
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("assignments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ContextId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.GoalId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.LearnerId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Work).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Version).IsConcurrencyToken();
            entity.HasIndex(x => new { x.TenantId, x.ContextId });
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.ToTable("submissions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasMaxLength(64);
            entity.Property(x => x.AssignmentId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.LearnerId).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.HasIndex(x => new { x.TenantId, x.AssignmentId });
        });
    }
}
