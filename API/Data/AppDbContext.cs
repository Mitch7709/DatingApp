using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<MemberLike> Likes => Set<MemberLike>();
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
