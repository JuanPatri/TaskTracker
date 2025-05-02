using Backend.Domain;
using Backend.Repository;

namespace Backend.Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    
    public ProjectService(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
    }
}