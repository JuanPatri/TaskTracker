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
            .Include(p => p.Tasks) 
                .ThenInclude(t => t.Resources)
                .ThenInclude(tr => tr.Resource)
                .ThenInclude(r => r.Type) 
            .Include(p => p.Tasks) 
                .ThenInclude(t => t.Dependencies)
                .ThenInclude(td => td.Dependency)
            .Include(p => p.ExclusiveResources) 
                .ThenInclude(r => r.Type) 
            .FirstOrDefault(predicate);
    }
    
    public IList<Project> FindAll()
    {
        return _sqlContext.Projects
            .Include(p => p.ProjectRoles)
                .ThenInclude(pr => pr.User)
            .Include(p => p.Tasks)  
                .ThenInclude(t => t.Resources)
                .ThenInclude(tr => tr.Resource)
                .ThenInclude(r => r.Type) 
            .Include(p => p.Tasks)  
                .ThenInclude(t => t.Dependencies)
                .ThenInclude(td => td.Dependency) 
            .Include(p => p.ExclusiveResources) 
                .ThenInclude(r => r.Type) 
            .ToList();
    }
    
    public Project? Update(Project updatedProject)
    {
        Project? existingProject = _sqlContext.Projects
            .Include(p => p.ProjectRoles)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Dependencies) 
            .Include(p => p.ExclusiveResources) 
            .FirstOrDefault(p => p.Id == updatedProject.Id);
            
        if (existingProject != null)
        {
            existingProject.Name = updatedProject.Name;
            existingProject.Description = updatedProject.Description;
            existingProject.StartDate = updatedProject.StartDate;

            existingProject.ProjectRoles.Clear();
            foreach (var role in updatedProject.ProjectRoles)
            {
                role.Project = existingProject; 
                existingProject.ProjectRoles.Add(role);
            }
            
            if (updatedProject.Tasks != null)
            {
                foreach (var task in updatedProject.Tasks)
                {
                    if (!existingProject.Tasks.Any(t => t.Title == task.Title))
                    {
                        existingProject.Tasks.Add(task);
                    }
                }
            }
        
            _sqlContext.SaveChanges();
            return existingProject;
        }
        return null;
    }
    
    public void Delete(String name)
    {
        Project? project = _sqlContext.Projects
            .Include(p => p.ProjectRoles)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Dependencies)
            .Include(p => p.ExclusiveResources) 
            .FirstOrDefault(p => p.Id == int.Parse(name));
            
        if (project != null)
        {
            _sqlContext.Projects.Remove(project);
            _sqlContext.SaveChanges();
        }
    }
}