using Domain;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    //public DbSet<Task> Tasks { get; set; }
    //public DbSet<Resource> Resources { get; set; }
    //public DbSet<Project> Projects { get; set; }
    //public DbSet<ResourceType> ResourceTypes { get; set; }
    //public DbSet<Notification> Notifications { get; set; }
    
    
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
        this.Database.Migrate();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Email);
            
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.BirthDate)
                .IsRequired();

            entity.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(u => u.Admin)
                .IsRequired()
                .HasDefaultValue(false);
        });
    }
}