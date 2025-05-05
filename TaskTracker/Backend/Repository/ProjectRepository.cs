using Backend.Domain;

namespace Backend.Repository;

public class ProjectRepository : IRepository<Project>
{
    private readonly List<Project> _projects;
    
    public ProjectRepository()
    {
        _projects = new List<Project>()
        {
            new Project()
            {
                Id = 1,
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
        Project? existingProject = _projects.FirstOrDefault(p => p.Id == updatedProject.Id);
        if (existingProject != null)
        {
            existingProject.Name = updatedProject.Name;
            existingProject.Description = updatedProject.Description;
            existingProject.StartDate = updatedProject.StartDate;
            existingProject.FinishDate = updatedProject.FinishDate;
            existingProject.Administrator = updatedProject.Administrator;
            return existingProject;
        }
        return null;
    }
    
    public void Delete(String name)
    {
        Project? project = _projects.FirstOrDefault(p => p.Id == int.Parse(name));
        if (project != null)
        {
            _projects.Remove(project);
        }
    }
}