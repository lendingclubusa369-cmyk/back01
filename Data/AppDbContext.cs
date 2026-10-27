using Microsoft.EntityFrameworkCore;
using lender_club.Models;

namespace lender_club.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LoanApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LoanAmount).HasPrecision(18, 2);
            entity.Property(e => e.AnnualIncome).HasPrecision(18, 2);
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
