using System.Xml;
using Domain;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<TaskDependency> TaskDependencies { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceType> ResourceTypes { get; set; }
    public DbSet<TaskResource> TaskResources { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
        //this.Database.Migrate();
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
        
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Description)
                .HasMaxLength(400);

            entity.Property(p => p.StartDate)
                .IsRequired();

            entity.HasMany(p => p.Tasks)
                .WithOne() 
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.ExclusiveResources)
                .WithOne() 
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(p => p.CriticalPath);
        });

        modelBuilder.Entity<ProjectRole>(entity =>
        {
            entity.HasKey(pr => pr.Id); 
    
            entity.Property(pr => pr.Id)
                .ValueGeneratedOnAdd(); 
            
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
        
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(t => t.Title); 
        
            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(1000);
            
            entity.Property(t => t.Duration)
                .IsRequired();
            
            entity.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();
            
            entity.Property(t => t.EarlyStart)
                .IsRequired();
            
            entity.Property(t => t.EarlyFinish)
                .IsRequired();
            
            entity.Property(t => t.LateStart)
                .IsRequired();
            
            entity.Property(t => t.LateFinish)
                .IsRequired();
            
            entity.Property(t => t.DateCompleated);

            entity.Property<int?>("ProjectId");
            
            entity.HasMany(t => t.Resources)
                .WithOne(tr => tr.Task)
                .HasForeignKey("TaskTitle")
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(t => t.Dependencies)
                .WithOne(td => td.Task)
                .HasForeignKey("TaskTitle")
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<TaskDependency>(entity =>
        {
            entity.HasKey(td => td.Id);
            
            entity.Property(td => td.Id)
                .ValueGeneratedOnAdd();
            
            entity.HasOne(td => td.Task)
                .WithMany(t => t.Dependencies)
                .HasForeignKey("TaskTitle")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            entity.HasOne(td => td.Dependency)
                .WithMany()
                .HasForeignKey("DependencyTitle")
                .OnDelete(DeleteBehavior.Restrict) 
                .IsRequired();
            
            entity.Property<string>("TaskTitle").HasMaxLength(200);
            entity.Property<string>("DependencyTitle").HasMaxLength(200);
            
            entity.HasIndex("TaskTitle", "DependencyTitle").IsUnique();
        });
        
        modelBuilder.Entity<TaskResource>(entity =>
        {
            entity.HasKey(tr => tr.Id);
            
            entity.Property(tr => tr.Id)
                .ValueGeneratedOnAdd();
        
            entity.Property(tr => tr.Quantity)
                .IsRequired();
            
            entity.HasOne(tr => tr.Task)
                .WithMany(t => t.Resources)
                .HasForeignKey("TaskTitle")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            
            entity.HasOne(tr => tr.Resource)
                .WithMany()
                .HasForeignKey("ResourceId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            entity.Property<string>("TaskTitle").HasMaxLength(200);
            entity.Property<int>("ResourceId");
            
            entity.HasIndex("TaskTitle", "ResourceId").IsUnique();
        });
        
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(r => r.Id);
            
            entity.Property(r => r.Id)
                .ValueGeneratedOnAdd();
        
            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(r => r.Description)
                .HasMaxLength(500);
            
            entity.Property(r => r.Quantity)
                .IsRequired();
            
            entity.Property<int?>("ProjectId");
            
            entity.HasOne(r => r.Type)
                .WithMany()
                .HasForeignKey("ResourceTypeId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
        
        modelBuilder.Entity<ResourceType>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Id)
                .ValueGeneratedOnAdd();
        
            entity.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(rt => rt.Name)
                .IsUnique();
        });
        
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.Id).ValueGeneratedOnAdd();

            entity.Property(n => n.Message).IsRequired();

            entity.Property(n => n.Date).IsRequired();

            entity.Property(n => n.TypeOfNotification)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(n => n.Impact).IsRequired();

            entity.HasMany(n => n.Users)
                .WithMany();

            entity.HasMany(n => n.Projects)
                .WithMany();

            entity.Property(n => n.ViewedBy)
                .HasConversion(
                    v => string.Join(",", v),                  
                    v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList() 
                );
        });
    }
}