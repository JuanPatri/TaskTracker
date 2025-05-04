using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Backend.Repository;

namespace Backend.Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    
    public ProjectService(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }
    
    public Project AddProject(ProjectDataDTO project)
    {
        Project createdProject = project.ToEntity();
        return _projectRepository.Add(createdProject);
    }
    
    public void RemoveProject(GetProjectDTO project)
    {
        _projectRepository.Delete(project.Name);
    }
    
    public Project? GetProject(GetProjectDTO project)
    {
        return _projectRepository.Find(p => p.Name == project.Name);
    }
}