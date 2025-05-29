using Domain;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class SqlContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Project> Projects { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
        this.Database.Migrate();
}
}