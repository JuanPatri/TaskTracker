using Backend.Domain;
using Backend.Domain.Enums;
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
    private int _idResourceType;
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    public readonly IRepository<User> _userRepository;
    public readonly IRepository<Notification> _notificationRepository;
    private int _notificationId = 1;
    public ProjectDataDTO? SelectedProject { get; set; }

    public ProjectService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository,
        IRepository<User> userRepository, IRepository<Notification> notificationRepository)
    {
        _projectRepository = projectRepository;
        _idProject = 2;
        _idResourceType = 4;
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }


    #region Project

    public Project AddProject(ProjectDataDTO project)
    {
        ValidateProjectName(project.Name);

        List<User> associatedUsers = GetUsersFromEmails(project.Users);

        if (!project.Users.Contains(project.Administrator.Email))
        {
            associatedUsers.Add(_userRepository.Find(u => u.Email == project.Administrator.Email));
        }

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

    public List<UserDataDTO> GetAllUsers()
    {
        return _userRepository.FindAll()
            .Select(user => new UserDataDTO
            {
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                BirthDate = user.BirthDate,
                Admin = user.Admin
            })
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
        if (string.IsNullOrWhiteSpace(projectDto.Administrator.Password))
        {
            User? fullAdmin = _userRepository.Find(u => u.Email == projectDto.Administrator.Email);
            projectDto.Administrator.Password = fullAdmin?.Password ?? "";
        }

        List<User> users = _userRepository.FindAll()
            .Where(u => projectDto.Users.Contains(u.Email))
            .ToList();

        Project? updatedProject = _projectRepository.Update(Project.FromDto(projectDto, users));
        return updatedProject;
    }

    public List<Task> GetTaskDependenciesWithTitleTask(List<string> titlesTask)
    {
        return _taskRepository.FindAll()
            .Where(t => titlesTask.Contains(t.Title))
            .ToList();
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
            .Where(ProjectHasUserAdmin(userEmail));

        return filteredProjects.Select(ToGetProjectDTO).ToList();
    }

    private Func<Project, bool> ProjectHasUserAdmin(string userEmail) =>
        project => project.Users != null
                   && project.Users.Any(user => project.Administrator.Email == user.Email);

    public List<GetProjectDTO> GetProjectsByUserEmailNotAdmin(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(project => project.Users != null && project.Users.Any(user => user.Email == userEmail)
                                                    && project.Administrator.Email != userEmail);

        return filteredProjects.Select(ToGetProjectDTO).ToList();
    }

    private GetProjectDTO ToGetProjectDTO(Project project) => new GetProjectDTO
    {
        Id = project.Id,
        Name = project.Name
    };

    public void AddExclusiveResourceToProject(int projectId, ResourceDataDto resourceDto)
    {
        Project project = _projectRepository.Find(p => p.Id == projectId);

        if (project == null)
            throw new ArgumentException($"No se encontró un proyecto con el ID {projectId}.");

        Resource newResource = new Resource
        {
            Name = resourceDto.Name,
            Description = resourceDto.Description,
            Type = _resourceTypeRepository.Find(r => r.Id == resourceDto.TypeResource)
        };

        project.ExclusiveResources.Add(newResource);

        _projectRepository.Update(project);
    }

    public List<ProjectDataDTO> GetAllProjectsDTOs()
    {
        return GetAllProjects()
            .Select(p => new ProjectDataDTO
            {
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                FinishDate = p.FinishDate,
                Administrator = new UserDataDTO
                {
                    Name = p.Administrator.Name,
                    LastName = p.Administrator.LastName,
                    Email = p.Administrator.Email
                }
            })
            .ToList();
    }

    public List<GetResourceDto> GetExclusiveResourcesForProject(int projectId)
    {
        Project? project = _projectRepository.Find(p => p.Id == projectId);

        if (project == null || project.ExclusiveResources == null)
            return new List<GetResourceDto>();

        return project.ExclusiveResources
            .Select(r => new GetResourceDto { Name = r.Name })
            .ToList();
    }

    public void DecreaseResourceQuantity(int projectId, string resourceName)
    {
        Project project = _projectRepository.Find(p => p.Id == projectId);
        if (project == null)
            throw new ArgumentException("Project not found");

        bool updated = false;

        foreach (var task in project.Tasks)
        {
            for (int i = 0; i < task.Resources.Count; i++)
            {
                var (qty, resource) = task.Resources[i];
                if (resource.Name == resourceName && qty > 0)
                {
                    task.Resources[i] = (qty - 1, resource);
                    updated = true;
                    break;
                }
            }

            if (updated)
                break;
        }

        if (!updated)
            throw new InvalidOperationException("Resource not found or quantity is already 0");

        _projectRepository.Update(project);
    }

    public List<ProjectDataDTO> ProjectsDataByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(project => project.Users != null && project.Users.Any(user => user.Email == userEmail));

        return filteredProjects.Select(ToProjectDataDto).ToList();
    }

    private ProjectDataDTO ToProjectDataDto(Project project) => new ProjectDataDTO
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        StartDate = project.StartDate,
        FinishDate = project.FinishDate,
        Administrator = new UserDataDTO
        {
            Name = project.Administrator.Name,
            LastName = project.Administrator.LastName,
            Email = project.Administrator.Email
        },
        Users = project.Users.Select(u => u.Email).ToList()
    };
    
    public DateTime GetEstimatedProjectFinishDate(Project project)
    {
        CalculateEarlyTimes(project);
        return project.Tasks.Max(t => t.EarlyFinish);
    }


    #endregion

    #region Task

    public Task AddTask(TaskDataDTO taskDto)
    {
        if (_taskRepository.Find(t => t.Title == taskDto.Title) != null)
        {
            throw new Exception("Task already exists");
        }

        List<Task> dependencies = GetTaskDependenciesWithTitleTask(taskDto.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(taskDto.Resources);

        Task createdTask = Task.FromDto(taskDto, resourceList, dependencies);
        Console.WriteLine("Dependencies:");
        foreach (var dep in createdTask.Dependencies)
        {
            Console.WriteLine($" - {dep.Title}");
        }

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
        List<Task> dependencies = GetTaskDependenciesWithTitleTask(taskDto.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(taskDto.Resources);
        
        return _taskRepository.Update(Task.FromDto(taskDto, resourceList, dependencies));
    }

    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);

        foreach (var project in _projectRepository.FindAll())
        {
            project.Tasks.RemoveAll(projTask => projTask.Title == task.Title);
        }
    }

    public List<GetTaskDTO> GetTasksForProjectWithId(int projectId)
    {
        Project? project = GetProjectById(projectId);
        if (project == null) return new List<GetTaskDTO>();

        return project.Tasks
            .Select(task => new GetTaskDTO { Title = task.Title })
            .ToList();
    }

    public Project? GetProjectById(int projectId)
    {
        return _projectRepository.Find(project => project.Id == projectId);
    }

    public void AddTaskToProject(TaskDataDTO taskDto, int projectId)
    {
        Project? project = _projectRepository.Find(p => p.Id == projectId);

        if (project == null)
        {
            throw new ArgumentException($"No project found with the ID {projectId}.");
        }

        Task? taskInRepository = _taskRepository.Find(t => t.Title == taskDto.Title);

        if (taskInRepository == null)
        {
            throw new InvalidOperationException("Task must be added to repository before linking to project.");
        }

        if (!project.Tasks.Contains(taskInRepository))
        {
            project.Tasks.Add(taskInRepository);
        }

        _projectRepository.Update(project);
    }


    public bool ValidateTaskStatus(string title, Status status)
    {
        List<Task> taskDependencies = GetTaskDependenciesWithTitleTask(new List<string> { title });

        return taskDependencies.Count < 0 || status == Status.Pending;
    }


    public void CalculateEarlyTimes(Project project)
    {
        var orderedTasks = GetTopologicalOrder(project.Tasks);

        foreach (var task in project.Tasks)
        {
            task.EarlyStart = default;
            task.EarlyFinish = default;
        }

        foreach (var task in orderedTasks)
        {
            DateTime es;

            if (task.Dependencies == null || task.Dependencies.Count == 0)
            {
                es = project.StartDate.ToDateTime(new TimeOnly(0, 0));
            }
            else
            {
                es = task.Dependencies
                    .Max(dep => dep.Status == Status.Completed && dep.DateCompleated.HasValue
                        ? dep.DateCompleated.Value
                        : dep.EarlyFinish);
            }

            if (task.Status == Status.Completed && task.DateCompleated.HasValue)
            {
                task.EarlyFinish = task.DateCompleated.Value;
                task.EarlyStart = task.DateCompleated.Value.AddDays(-task.Duration);
            }
            else
            {
                task.EarlyStart = es;
                task.EarlyFinish = es.AddDays(task.Duration);
            }
        }
    }



    private List<Task> GetTopologicalOrder(List<Task> tasks)
    {
        var visited = new HashSet<Task>();
        var tempVisited = new HashSet<Task>();
        var result = new List<Task>();

        foreach (var task in tasks)
        {
            if (!visited.Contains(task))
            {
                TopologicalSortDFS(task, visited, tempVisited, result);
            }
        }

        return result;
    }

    private void TopologicalSortDFS(Task task, HashSet<Task> visited, HashSet<Task> tempVisited, List<Task> result)
    {
        if (tempVisited.Contains(task))
        {
            throw new InvalidOperationException("Cycle detected in task dependencies");
        }

        if (visited.Contains(task))
        {
            return;
        }

        tempVisited.Add(task);


        if (task.Dependencies != null)
        {
            foreach (var dependency in task.Dependencies)
            {
                TopologicalSortDFS(dependency, visited, tempVisited, result);
            }
        }

        tempVisited.Remove(task);
        visited.Add(task);
        result.Add(task);
    }
    
    public bool CanMarkTaskAsCompleted(TaskDataDTO dto)
    {
        var task = _taskRepository.Find(t => t.Title == dto.Title);
        if (task == null) return false;

        return task.Dependencies.All(d => d.Status == Status.Completed);
    }
    
    public void CalculateLateTimes(Project project)
    {
        CalculateEarlyTimes(project);
        
                
        if (project.Tasks == null || !project.Tasks.Any())
            throw new InvalidOperationException("The project has no defined tasks.");
        
        DateTime maxEF = project.Tasks.Max(t => t.EarlyFinish);

        foreach (var task in project.Tasks)
        {
            task.LateFinish = maxEF;
            task.LateStart = maxEF.AddDays(-task.Duration);
        }

        List<Task> ordered = GetTopologicalOrder(project.Tasks);
        ordered.Reverse();

        foreach (var task in ordered)
        {
            List<Task> successors = project.Tasks
                .Where(t => t.Dependencies.Contains(task))
                .ToList();

            if (successors.Any())
            {
                task.LateFinish = successors.Min(s => s.LateStart);
                task.LateStart = task.LateFinish.AddDays(-task.Duration);
            }
        }
    }

    public List<Task> GetCriticalPath(Project project)
    {
        CalculateLateTimes(project);

        var criticalTasks = project.Tasks
            .Where(t => t.EarlyStart == t.LateStart)
            .ToHashSet();

        var startTasks = project.Tasks
            .Where(t => t.Dependencies == null || t.Dependencies.Count == 0)
            .Where(t => criticalTasks.Contains(t))
            .ToList();

        foreach (var start in startTasks)
        {
            var path = new List<Task>();
            if (BuildCriticalPath(start, project.Tasks, criticalTasks, path))
            {
                return path;
            }
        }

        return new List<Task>(); 
    }

    private bool BuildCriticalPath(Task current, List<Task> allTasks, HashSet<Task> criticalTasks, List<Task> path)
    {
        path.Add(current);

        var successors = allTasks
            .Where(t => t.Dependencies.Contains(current))
            .Where(t => criticalTasks.Contains(t))
            .ToList();

        if (successors.Count == 0)
        {
            return true; 
        }

        foreach (var next in successors)
        {
            if (BuildCriticalPath(next, allTasks, criticalTasks, path))
                return true;
        }

        path.Remove(current); 
        return false;
    }

    public string? GetAdminEmailByTaskTitle(string title)
    {
        Project? projectWithTask = _projectRepository.Find(p => p.Tasks.Any(t => t.Title == title));
        return projectWithTask?.Administrator.Email;
    }
    
    #endregion

    #region Resource

    public Resource? AddResource(ResourceDataDto resource)
    {
        if (_resourceRepository.Find(r => r.Name == resource.Name) != null)
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

    public List<GetResourceDto> GetResourcesForSystem()
    {
        return _resourceRepository.FindAll()
            .Select(resource => new GetResourceDto { Name = resource.Name })
            .ToList();
    }

    #endregion

    #region Notification
    
    public bool IsTaskCritical(Project project, string taskTitle)
    {
        
        if (project == null || project.Tasks == null)
        {
            return false;
        }
        var task = project.Tasks?.FirstOrDefault(t => t.Title == taskTitle);
        if (task == null)
        {
            return false;
        }
        var criticalPath = GetCriticalPath(project);
        return criticalPath.Any(task => task.Title.Equals(taskTitle));
    }
    
    public TypeOfNotification ObtenerTipoDeNotificacionPorImpacto(int impacto)
    {
        if (impacto > 0)
            return TypeOfNotification.Delay;
        else
            return TypeOfNotification.DurationAdjustment;
    }

    public int CalcularImpacto(int duracionVieja, int duracionNueva)
    {
        return duracionNueva - duracionVieja;
    }

    public DateTime GetNewEstimatedEndDate(int projectId)
    {
        var proyecto = GetProjectById(projectId);
        if (proyecto == null)
            throw new ArgumentException("Proyecto no encontrado");

        return GetEstimatedProjectFinishDate(proyecto);
    }

    public List<User> GetUsersFromProject(int projectId)
    {
        var project = GetProjectById(projectId);
        if (project == null || project.Users == null)
            return new List<User>();
        return project.Users;
    }

    public string GenerateNotificationMessage(TypeOfNotification type, string taskTitle, DateTime newEstimatedEndDate)
    {
        switch (type)
        {
            case TypeOfNotification.Delay:
                return $"The critical task '{taskTitle}' has caused a delay. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
            case TypeOfNotification.DurationAdjustment:
                return $"The duration of the critical task '{taskTitle}' was adjusted. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
            default:
                return $"The task '{taskTitle}' has had a change. The new estimated project end date is {newEstimatedEndDate:yyyy-MM-dd}.";
        }
    }
    public Notification CreateNotification(int duracionVieja, int duracionNueva, int projectId, string taskTitle)
    {
        int impacto = CalcularImpacto(duracionVieja, duracionNueva);
        TypeOfNotification tipo = ObtenerTipoDeNotificacionPorImpacto(impacto);
        DateTime nuevaFechaFin = GetNewEstimatedEndDate(projectId);
        List<User> users = GetUsersFromProject(projectId);
        string message = GenerateNotificationMessage(tipo, taskTitle, nuevaFechaFin);

        var notificacion = new Notification
        {
            Id = _notificationId++,
            Message = message,
            TypeOfNotification = tipo,
            Impact = impacto,
            Date = nuevaFechaFin,
            Users = users,
        };

        return _notificationRepository.Add(notificacion);
    }

    public List<Notification> GetNotificationsForUser(string email)
    {
        return _notificationRepository
            .FindAll()
            .Where(n => n.Users != null && n.Users.Any(u => u.Email == email))
            .Where(n => n.ViewedBy == null || !n.ViewedBy.Contains(email))
            .ToList();
    }
    
    public void MarkNotificationAsViewed(int notificationId, string userEmail)
    {
        var notification = _notificationRepository.Find(n => n.Id == notificationId);
        if (notification != null && !notification.ViewedBy.Contains(userEmail))
        {
            notification.ViewedBy.Add(userEmail);
            if (notification.Projects == null)
            {
                notification.Projects = new List<Project>();
            }
            _notificationRepository.Update(notification);
        }
    }
    
    public List<Notification> GetUnviewedNotificationsForUser(string email)
    {
        return _notificationRepository
            .FindAll()
            .Where(n => n.Users != null && n.Users.Any(u => u.Email == email) && !n.ViewedBy.Contains(email))
            .ToList();
    }
    #endregion

    #region ResourceType

    public ResourceType? AddResourceType(ResourceTypeDto resourceType)
    {
        if (_resourceTypeRepository.Find(r => r.Name == resourceType.Name) != null)
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