using Domain;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    //public DbSet<Task> Tasks { get; set; }
    //public DbSet<Resource> Resources { get; set; }
    public DbSet<Project> Projects { get; set; }
    //public DbSet<ResourceType> ResourceTypes { get; set; }
    //public DbSet<Notification> Notifications { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
        //this.Database.Migrate();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Task>();
        modelBuilder.Ignore<TaskResource>();
        modelBuilder.Ignore<Resource>();
        modelBuilder.Ignore<ResourceType>();
        
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
        
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Description)
                .HasMaxLength(400);

            entity.Property(p => p.StartDate)
                .IsRequired();

            entity.HasOne(p => p.Administrator)
                .WithMany()
                .HasForeignKey("AdministratorEmail")
                .IsRequired();
            
            entity.Ignore(p => p.Tasks);
        });
        
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Users)
            .WithMany(u => u.Projects)
            .UsingEntity(j => j.ToTable("UserProject"));
    }
}