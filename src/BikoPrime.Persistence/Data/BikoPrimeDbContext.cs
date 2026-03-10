namespace BikoPrime.Persistence.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BikoPrime.Domain.Entities;

public class BikoPrimeDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public BikoPrimeDbContext(DbContextOptions<BikoPrimeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(entity =>
        {
            entity.Property(u => u.AvatarUrl).HasMaxLength(500);
            entity.Property(u => u.Address).HasMaxLength(500);
            entity.Property(u => u.Bio).HasMaxLength(500);
            entity.Property(u => u.Rating).HasPrecision(3, 2);
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(1000);
            entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);
            
            entity.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => rt.UserId);
        });
    }
}
