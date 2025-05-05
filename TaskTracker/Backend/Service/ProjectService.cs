using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Backend.Repository;

namespace Backend.Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private int _id;
    
    public ProjectService(IRepository<Project> projectRepository)
    {
        _projectRepository = projectRepository;
        _id = 2;
    }
    
    public Project AddProject(ProjectDataDTO project)
    {
        if (_projectRepository.Find(p => p.Name == project.Name) != null)
        {
            throw new ArgumentException("Project with the same name already exists");
        }
        project.Id = _id++;
        Project? createdProject = _projectRepository.Add(project.ToEntity());
        return createdProject;
    }
    
    public void RemoveProject(GetProjectDTO project)
    {
        _projectRepository.Delete(project.Id.ToString());
    }
    
    public Project? GetProject(GetProjectDTO project)
    {
        return _projectRepository.Find(p => p.Id == project.Id);
    }
    
    public List<Project> GetAllProjects()
    {
        return _projectRepository.FindAll().ToList();
    }
    
    public Project? UpdateProject(ProjectDataDTO projectDto)
    {
        Project? updatedProject = _projectRepository.Update(projectDto.ToEntity());
        return updatedProject;
    }
}