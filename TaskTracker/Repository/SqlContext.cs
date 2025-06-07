using Domain;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    
    //public DbSet<Task> Tasks { get; set; }
    //public DbSet<Resource> Resources { get; set; }
    //public DbSet<ResourceType> ResourceTypes { get; set; }
    //public DbSet<TaskResource> TaskResources { get; set; }
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
            
            entity.Ignore(p => p.Tasks);
            entity.Ignore(p => p.ExclusiveResources);
            entity.Ignore(p => p.CriticalPath);
        });

        modelBuilder.Entity<ProjectRole>(entity =>
        {
            entity.HasKey("ProjectId", "UserEmail");
            
            entity.Property(pr => pr.RoleType)
                .IsRequired()
                .HasConversion<string>();

            entity.HasOne(pr => pr.Project)
                .WithMany(p => p.ProjectRoles)
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey("UserEmail")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Property<int>("ProjectId");
            entity.Property<string>("UserEmail");
        });
    }
}