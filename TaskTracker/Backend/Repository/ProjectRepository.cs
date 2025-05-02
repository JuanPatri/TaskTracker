using Backend.Domain;

namespace Backend.Repository;

public class ProjectRepository 
{
    private readonly List<Project> _projects;
    
    public ProjectRepository()
    {
        _projects = new List<Project>()
        {
            new Project()
            {
                Name = "Project1",
                Description = "Description1",
                StartDate = DateTime.Now.AddDays(1),
                FinishDate = DateTime.Now.AddYears(1),
                Administrator = new User()
                {
                    Name = "Admin",
                    LastName = "Admin",
                    Email = "admin@admin.com",
                    Password = "Admin123@",
                    Admin = true,
                    BirthDate = new DateTime(1990, 1, 1)
                }
            }
        };
    }
    public Project Add(Project project)
    {
        _projects.Add(project);
        return project;
    }
    
    public Project? Find(Func<Project, bool> predicate)
    {
        return _projects.FirstOrDefault(predicate);
    }
    
    public IList<Project> FindAll()
    {
        return _projects;
    }
    
    public Project? Update(Project updatedProject)
    {
        throw new NotImplementedException();
    }
}