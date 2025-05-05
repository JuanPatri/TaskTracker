using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Backend.DTOs.ResourceDTOs;
using Backend.Repository;
using Task = Backend.Domain.Task;
using Backend.DTOs.TaskDTOs;

namespace Backend.Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private int _id;
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    
    public ProjectService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository)
    {
        _projectRepository = projectRepository;
        _id = 2;
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
    }

    
    #region Project
    public Project AddProject(ProjectDataDTO project)
    {
        if (_projectRepository.Find(p => p.Name == project.Name) != null)
        {
            throw new ArgumentException("Project with the same name already exists");
        }
        project.Id = _id++;
        Project? createdProject = _projectRepository.Add(Project.FromDto(project));
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
        Project? updatedProject = _projectRepository.Update(Project.FromDto(projectDto));
        return updatedProject;
    }
    
    public List<Task> GetTaskDependenciesWithTitle(List<string> titlesTask)
    {
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => t.Dependencies.Any(dependency => titlesTask.Contains(dependency.Title)))
            .ToList();

        return taskDependencies;
    }
    public List<(int, Resource)> GetResourcesWithName(List<(int, string)> namesResource)
    {
        List<(int, Resource)> resourceList = namesResource
            .Select(tuple =>
            {
                int cantidad = tuple.Item1;
                string nameResource = tuple.Item2;

                Resource? resource = _resourceRepository.Find(r => r.Name == nameResource);
                return (cantidad, resource);
            })
            .Where(t => t.Item2 != null)
            .Select(t => (t.Item1, t.Item2!))
            .ToList();

        return resourceList;
    }
    
    #endregion
    
    #region Task
    public Task AddTask(TaskDataDTO taskDto)
    {
        
        List<Task> taskDependencies = GetTaskDependenciesWithTitle(taskDto.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(taskDto.Resources);

        Task createdTask = Task.FromDto(taskDto, taskDependencies, resourceList);
        return _taskRepository.Add(createdTask);
    }
    public Task GetTaskByTitle(string title)
    {
        return _taskRepository.Find(t => t.Title == title);
    }
    public List<Task> GetAllTasks()
    {
        return _taskRepository.FindAll().ToList();
    }
    public Task? UpdateTask(TaskDataDTO taskDto)
    {
        List<Task> taskDependencies = GetTaskDependenciesWithTitle(taskDto.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(taskDto.Resources);

        return _taskRepository.Update(Task.FromDto(taskDto, taskDependencies, resourceList));
    }
    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);
    }
    #endregion

    #region Resource
    public Resource? AddResource(ResourceDataDto resource)
    {
        if(_resourceRepository.Find(r => r.Name == resource.Name) != null)
        {
            throw new Exception("Resource already exists");
        }
        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resource.TypeResource);
        Resource? createdResource = _resourceRepository.Add(Resource.FromDto(resource, resourceType));
        return createdResource;
    }
    
    public void RemoveResource(GetResourceDto resource)
    {
        _resourceRepository.Delete(resource.Name);
    }
    
    public Resource? GetResource(GetResourceDto resource)
    {
        return _resourceRepository.Find(r => r.Name == resource.Name);
    }
    
    public List<Resource> GetAllResources()
    {
        return _resourceRepository.FindAll().ToList();
    }
    
    public Resource? UpdateResource(ResourceDataDto resourceDto)
    {
        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resourceDto.TypeResource);
        Resource? updatedResource = _resourceRepository.Update(Resource.FromDto(resourceDto, resourceType));
        return updatedResource;
    }
    #endregion
}