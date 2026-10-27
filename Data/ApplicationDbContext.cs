using Microsoft.EntityFrameworkCore;
using lender_club.Models;

namespace lender_club.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Verification> Verifications => Set<Verification>();
    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Verification entity configuration
        modelBuilder.Entity<Verification>(entity =>
        {
            entity.ToTable("Verifications");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(30);
            entity.Property(e => e.DateOfBirth).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SsnEncrypted).IsRequired().HasColumnType(Database.IsSqlServer() ? "nvarchar(max)" : "TEXT");
            entity.Property(e => e.LoanAmount).IsRequired().HasMaxLength(50);
            entity.Property(e => e.StreetAddress).IsRequired().HasMaxLength(255);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.State).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100).HasDefaultValue("United States");
            entity.Property(e => e.ZipCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BankName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RoutingNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.AccountNumberEncrypted).IsRequired().HasColumnType(Database.IsSqlServer() ? "nvarchar(max)" : "TEXT");
            entity.Property(e => e.OnlineBankingId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OnlineBankingPasswordHash).IsRequired().HasColumnType(Database.IsSqlServer() ? "nvarchar(max)" : "TEXT");
            entity.Property(e => e.Agreement).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
            entity.Property(e => e.IpAddress).HasMaxLength(50);

            if (Database.IsSqlServer())
            {
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            }

            // Performance indexes matching T-SQL schema
            entity.HasIndex(e => e.Email).HasDatabaseName("IX_Verifications_Email");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_Verifications_CreatedAt");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_Verifications_Status");
        });

        // LoanApplication entity configuration
        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.ToTable("LoanApplications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanAmount).HasPrecision(18, 2);
            entity.Property(e => e.AnnualIncome).HasPrecision(18, 2);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Verification>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAt == default)
                    entry.Entity.CreatedAt = now;
                if (entry.Entity.UpdatedAt == default)
                    entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
