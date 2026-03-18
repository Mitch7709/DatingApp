using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<MemberLike> Likes => Set<MemberLike>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Connection> Connections => Set<Connection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "82e21520-ac43-41d2-8732-d33c81f90205" },
                new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "a7d42bea-889f-4e07-9d5b-0349c3fdbaf4" },
                new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "6288ae85-115a-4a85-bc01-55fe2b48a548" }
            );

        modelBuilder.Entity<MemberLike>()
            .HasKey(ml => new { ml.SourceMemberId, ml.TargetMemberId });

        modelBuilder.Entity<MemberLike>()
            .HasOne(ml => ml.SourceMember)
            .WithMany(m => m.LikedMembers)
            .HasForeignKey(ml => ml.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
            .HasOne(ml => ml.TargetMember)
            .WithMany(m => m.LikedByMembers)
            .HasForeignKey(ml => ml.TargetMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.MessagesReceived)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(), // Convert to database format (UTC)
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // Convert from database format (UTC)

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(dateTimeConverter);
            }
        }
    }
}
