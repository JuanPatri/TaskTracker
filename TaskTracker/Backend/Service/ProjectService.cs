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
}