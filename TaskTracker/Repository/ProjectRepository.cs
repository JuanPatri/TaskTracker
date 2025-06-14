using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ProjectRepository : IRepository<Project>
{
    private readonly SqlContext _sqlContext;
    
    public ProjectRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;
    }
    public Project Add(Project project)
    {
        _sqlContext.Projects.Add(project);
        _sqlContext.SaveChanges();
        return project;
    }
    
    public Project? Find(Func<Project, bool> predicate)
    {
        return _sqlContext.Projects
            .Include(p => p.ProjectRoles)
                .ThenInclude(pr => pr.User)
            .FirstOrDefault(predicate);
    }
    
    public IList<Project> FindAll()
    {
        return _sqlContext.Projects
            .Include(p => p.ProjectRoles)
                .ThenInclude(pr => pr.User)
            .ToList();
    }
    
    public Project? Update(Project updatedProject)
    {
        Project? existingProject = _sqlContext.Projects.FirstOrDefault(p => p.Id == updatedProject.Id);
        if (existingProject != null)
        {
            existingProject.Name = updatedProject.Name;
            existingProject.Description = updatedProject.Description;
            existingProject.StartDate = updatedProject.StartDate;
            
            existingProject.ProjectRoles.Clear();
            foreach (var role in updatedProject.ProjectRoles)
            {
                existingProject.ProjectRoles.Add(role);
            }
            
            // existingProject.Tasks = updatedProject.Tasks;
            // existingProject.ExclusiveResources = updatedProject.ExclusiveResources;
        
            _sqlContext.SaveChanges();
            return existingProject;
        }
        return null;
    }
    
    public void Delete(String name)
    {
        Project? project = _sqlContext.Projects.FirstOrDefault(p => p.Id == int.Parse(name));
        if (project != null)
        {
            _sqlContext.Projects.Remove(project);
            _sqlContext.SaveChanges();
        }
    }
}