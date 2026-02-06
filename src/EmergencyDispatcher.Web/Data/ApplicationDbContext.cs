using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmergencyDispatcher.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Member configuration
        builder.Entity<Member>(entity =>
        {
            entity.HasIndex(m => m.Phone);
            entity.HasIndex(m => m.FullName);

            entity.HasOne(m => m.PreferredHospital)
                .WithMany()
                .HasForeignKey(m => m.PreferredHospitalId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Hospital configuration
        builder.Entity<Hospital>(entity =>
        {
            entity.HasIndex(h => h.Name);
        });

        // Case configuration
        builder.Entity<Case>(entity =>
        {
            entity.HasIndex(c => c.Status);
            entity.HasIndex(c => c.CreatedAt);

            entity.HasOne(c => c.Member)
                .WithMany()
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(c => c.Hospital)
                .WithMany()
                .HasForeignKey(c => c.HospitalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasIndex(a => a.CaseId);
            entity.HasIndex(a => a.Timestamp);

            entity.HasOne(a => a.Case)
                .WithMany(c => c.AuditLogs)
                .HasForeignKey(a => a.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Performer)
                .WithMany()
                .HasForeignKey(a => a.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
