using Backend.Domain;
using Backend.DTOs.ProjectDTOs;
using Backend.DTOs.ResourceDTOs;
using Backend.DTOs.ResourceTypeDTOs;
using Backend.Repository;
using Task = Backend.Domain.Task;
using Backend.DTOs.TaskDTOs;
using Backend.DTOs.UserDTOs;

namespace Backend.Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private int _idProject;
    private int _idResourceType ;
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    public readonly IRepository<User> _userRepository;
    
    public ProjectService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository, IRepository<User> userRepository)
    {
        _projectRepository = projectRepository;
        _idProject = 2;
        _idResourceType = 4;
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _userRepository = userRepository;
    }

    
    #region Project
    public Project AddProject(ProjectDataDTO project)
    {
        ValidateProjectName(project.Name);

        var associatedUsers = GetUsersFromEmails(project.Users);

        project.Id = _idProject++;
        return _projectRepository.Add(Project.FromDto(project, associatedUsers));
    }

    private void ValidateProjectName(string projectName)
    {
        if (_projectRepository.Find(p => p.Name == projectName) != null)
        {
            throw new ArgumentException("Project with the same name already exists");
        }
    }

    private List<User> GetUsersFromEmails(IEnumerable<string> userEmails)
    {
        return _userRepository
            .FindAll()
            .Where(user => userEmails.Contains(user.Email))
            .ToList();
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
        List<User> users = _userRepository.FindAll()
            .Where(u => projectDto.Users.Contains(u.Email))
            .ToList();


        Project? updatedProject = _projectRepository.Update(Project.FromDto(projectDto, users));
        return updatedProject;
    }
    
    public List<Task> GetTaskDependenciesWithTitle(List<string> titlesTask)
    {
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => t.Dependencies.Any(dependency => titlesTask.Contains(dependency.Title)))
            .ToList();

        return taskDependencies;
    }
    public List<(int, Resource)> GetResourcesWithName(List<(int, string)> resourceName)
    {
        return resourceName
            .Select(tuple => (tuple.Item1, FindResourceByName(tuple.Item2)))
            .Where(t => t.Item2 is not null)
            .Select(t => (t.Item1, t.Item2!))
            .ToList();
    }

    private Resource? FindResourceByName(string resourceName)
    {
        return _resourceRepository.Find(resource => resource.Name == resourceName);
    }
    
    public List<GetProjectDTO> GetProjectsByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(ProjectHasUserAndIsAdmin(userEmail));

        return filteredProjects.Select(ToGetProjectDTO).ToList();
    }

    private static Func<Project, bool> ProjectHasUserAndIsAdmin(string userEmail) =>
        project => project.Users != null 
                   && project.Users.Any(user => user.Email == userEmail && project.Administrator == user);

    private static GetProjectDTO ToGetProjectDTO(Project project) => new GetProjectDTO
    {
        Id = project.Id,
        Name = project.Name
    };
    
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
    public List<GetTaskDTO> GetTasksForProjectWithId(int projectId)
    {
        Project? project = GetProjectById(projectId);
        if (project == null) return new List<GetTaskDTO>();

        return project.Tasks
            .Select(task => new GetTaskDTO { Title = task.Title })
            .ToList();
    }

    private Project? GetProjectById(int projectId)
    {
        return _projectRepository.Find(project => project.Id == projectId);
    }
    
    public void AddTaskToProject(TaskDataDTO taskDto, int projectId)
    {
        Project project = _projectRepository.Find(p => p.Id == projectId);        
        
        if (project == null)
        {
            throw new ArgumentException($"No se encontró un proyecto con el ID {projectId}.");
        }

        List<Task> taskDependencies = GetTaskDependenciesWithTitle(taskDto.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(taskDto.Resources);

        Task newTask = Task.FromDto(taskDto, taskDependencies, resourceList);

        project.Tasks.Add(newTask);

        _projectRepository.Update(project);
    }

    public List<UserIdentificationDTO> GetUsersForProjectWithId(int projectId)
    {
        Project? project = GetProjectById(projectId); 
        List<UserIdentificationDTO> usersProject = new List<UserIdentificationDTO>();
        
            foreach (User user in project.Users)
            {
                usersProject.Add(new UserIdentificationDTO
                {
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName
                });
            }
        return usersProject;
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

    #region ResourceType
    public ResourceType? AddResourceType(ResourceTypeDto resourceType)
    {
        if(_resourceTypeRepository.Find(r => r.Name == resourceType.Name) != null)
        {
            throw new Exception("Resource type already exists");
        }
        resourceType.Id = _idResourceType++;
        ResourceType? createdResourceType = _resourceTypeRepository.Add(ResourceType.Fromdto(resourceType));
        return createdResourceType;
    }
    
    public void RemoveResourceType(ResourceTypeDto resourceType)
    {
        _resourceTypeRepository.Delete(resourceType.Id.ToString());
    }
    
    public ResourceType? GetResourceType(ResourceTypeDto resourceType)
    {
        return _resourceTypeRepository.Find(r => r.Id == resourceType.Id);
    }
    
    public List<ResourceType> GetAllResourcesType()
    {
        return _resourceTypeRepository.FindAll().ToList();
    }
    
    public ResourceType? UpdateResourceType(ResourceTypeDto resourceTypeDto)
    {
        ResourceType? updatedResourceType = _resourceTypeRepository.Update(ResourceType.Fromdto(resourceTypeDto));
        return updatedResourceType;
    }
    #endregion
}