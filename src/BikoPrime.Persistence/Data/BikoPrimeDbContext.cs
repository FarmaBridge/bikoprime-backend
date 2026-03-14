namespace BikoPrime.Persistence.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BikoPrime.Domain.Entities;

public class BikoPrimeDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    
    public DbSet<Service> Services { get; set; } = null!;
    
    public DbSet<Demand> Demands { get; set; } = null!;
    
    public DbSet<Conversation> Conversations { get; set; } = null!;
    
    public DbSet<Message> Messages { get; set; } = null!;
    
    public DbSet<Contract> Contracts { get; set; } = null!;
    
    public DbSet<Review> Reviews { get; set; } = null!;
    
    public DbSet<UserFollow> UserFollows { get; set; } = null!;

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
            entity.Property(u => u.Street).HasMaxLength(100);
            entity.Property(u => u.StreetNumber).HasMaxLength(10);
            entity.Property(u => u.Complement).HasMaxLength(50);
            entity.Property(u => u.Neighborhood).HasMaxLength(50);
            entity.Property(u => u.City).HasMaxLength(50);
            entity.Property(u => u.State).HasMaxLength(2);
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

        builder.Entity<Service>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).IsRequired().HasMaxLength(500);
            entity.Property(s => s.Description).IsRequired().HasMaxLength(2000);
            entity.Property(s => s.Category).IsRequired().HasMaxLength(100);
            entity.Property(s => s.Subcategory).HasMaxLength(100);
            entity.Property(s => s.PriceType).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Price).HasPrecision(10, 2);
            entity.Property(s => s.Address).HasMaxLength(500);
            entity.Property(s => s.Rating).HasPrecision(3, 2);
            entity.Property(s => s.ServiceRadiusKm).HasPrecision(5, 2);

            entity.HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => s.ProviderId);
            entity.HasIndex(s => s.Category);
        });

        builder.Entity<Demand>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Title).IsRequired().HasMaxLength(500);
            entity.Property(d => d.Description).IsRequired().HasMaxLength(2000);
            entity.Property(d => d.Category).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Subcategory).HasMaxLength(100);
            entity.Property(d => d.Budget).HasPrecision(10, 2);
            entity.Property(d => d.Address).HasMaxLength(500);
            entity.Property(d => d.ServiceRadiusKm).HasPrecision(5, 2);

            entity.HasOne(d => d.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.CreatedBy);
            entity.HasIndex(d => d.Category);
        });

        builder.Entity<Conversation>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ParticipantIds).IsRequired();

            entity.HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(c => c.CreatedAt);
        });

        builder.Entity<Message>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Content).IsRequired().HasMaxLength(2000);
            entity.Property(m => m.Type).IsRequired().HasMaxLength(50);

            entity.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(m => m.ConversationId);
            entity.HasIndex(m => m.SenderId);
        });

        builder.Entity<Contract>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Status).IsRequired().HasMaxLength(50);

            entity.HasOne(c => c.Service)
                .WithMany(s => s.Contracts)
                .HasForeignKey(c => c.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Provider)
                .WithMany()
                .HasForeignKey(c => c.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.ServiceId);
            entity.HasIndex(c => c.ProviderId);
            entity.HasIndex(c => c.ClientId);
        });

        builder.Entity<Review>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).HasMaxLength(1000);

            entity.HasOne(r => r.Contract)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.TargetUser)
                .WithMany()
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => r.ContractId);
            entity.HasIndex(r => r.AuthorId);
            entity.HasIndex(r => r.TargetUserId);
        });

        builder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(uf => uf.Id);

            entity.HasOne(uf => uf.Follower)
                .WithMany()
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uf => uf.Following)
                .WithMany()
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(uf => new { uf.FollowerId, uf.FollowingId }).IsUnique();
        });
    }
}
