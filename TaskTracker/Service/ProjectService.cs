using Domain;
using DTOs.ProjectDTOs;
using DTOs.ResourceDTOs;
using DTOs.TaskDTOs;
using DTOs.UserDTOs;
using Enums;
using Repository;
using Task = Domain.Task;

namespace Service;

public class ProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private int _idProject;
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private readonly IRepository<User> _userRepository;
    public ProjectDataDTO? SelectedProject;
    private readonly UserService _userService;
    private readonly CriticalPathService _criticalPathService;
    private int _idResource;
    
    public ProjectService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<ResourceType> resourceTypeRepository,  IRepository<User> userRepository, UserService userService,
        CriticalPathService criticalPathService)
    {
        _projectRepository = projectRepository;
        _idProject = 2;
        _idResource = 1;
        _taskRepository = taskRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _userRepository = userRepository;
        _userService = userService;
        _criticalPathService = criticalPathService;
    }
    
    public Project AddProject(ProjectDataDTO project)
    {
        ValidateProjectName(project.Name);

        List<User> associatedUsers = _userService.GetUsersFromEmails(project.Users);
        
        List<ProjectRole> projectRoles = new List<ProjectRole>();
    
        for (int i = 0; i < associatedUsers.Count; i++)
        {
            User user = associatedUsers[i];
            RoleType roleType = (i == 0) ? RoleType.ProjectAdmin : RoleType.ProjectMember;
        
            ProjectRole role = new ProjectRole
            {
                RoleType = roleType,
                User = user
            };
            projectRoles.Add(role);
        }
    
        project.Id = _idProject++;
        
        Project newProject = Project.FromDto(project, projectRoles);
        
        foreach (var role in projectRoles)
        {
            role.Project = newProject;
        }
    
        return _projectRepository.Add(newProject);
    }

    private void ValidateProjectName(string projectName)
    {
        if (_projectRepository.Find(p => p.Name == projectName) != null)
        {
            throw new ArgumentException("Project with the same name already exists");
        }
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
        List<User> associatedUsers = _userRepository.FindAll()
            .Where(u => projectDto.Users.Contains(u.Email))
            .ToList();

        List<ProjectRole> projectRoles = new List<ProjectRole>();
    
        for (int i = 0; i < associatedUsers.Count; i++)
        {
            User user = associatedUsers[i];
            RoleType roleType = (i == 0) ? RoleType.ProjectAdmin : RoleType.ProjectMember;
        
            ProjectRole role = new ProjectRole
            {
                RoleType = roleType,
                User = user
            };
            projectRoles.Add(role);
        }

        Project? updatedProject = _projectRepository.Update(Project.FromDto(projectDto, projectRoles));
        
        if (updatedProject != null)
        {
            foreach (var role in updatedProject.ProjectRoles)
            {
                role.Project = updatedProject;
            }
        }
    
        return updatedProject;
    }

    public List<GetProjectDTO> GetProjectsByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(ProjectHasUserAdmin(userEmail));

        return filteredProjects.Select(ToGetProjectDTO).ToList();
    }

    private Func<Project, bool> ProjectHasUserAdmin(string userEmail) =>
        project => project.Administrator.Email == userEmail;

    public List<GetProjectDTO> GetAllProjectsByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(project =>
                project.Users != null &&
                project.Users.Any(user => user.Email == userEmail));

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
            Quantity = resourceDto.Quantity,
            Id = GetNextExclusiveResourceId(project.ExclusiveResources),
            Type = _resourceTypeRepository.Find(r => r.Id == resourceDto.TypeResource)
        };

        project.ExclusiveResources.Add(newResource);

        _projectRepository.Update(project);
    }
    
    private int GetNextExclusiveResourceId(List<Resource> resources)
    {
        int minExclusiveId = 1000;
    
        if (!resources.Any())
        {
            return minExclusiveId;
        }
    
        int maxId = resources.Max(r => r.Id);
        int nextId = maxId + 1;
    
        if (nextId < minExclusiveId)
        {
            nextId = minExclusiveId;
        }
    
        if (nextId >= 2000)
        {
            throw new InvalidOperationException("Too many exclusive resources. Max 999 exclusive resources allowed.");
        }
    
        return nextId;
    }

    public List<ProjectDataDTO> GetAllProjectsDTOs()
    {
        return GetAllProjects()
            .Select(p => new ProjectDataDTO
            {
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
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
            .Select(r => new GetResourceDto 
            { 
                ResourceId = r.Id,  
                Name = r.Name 
            })
            .ToList();
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
       _criticalPathService.CalculateEarlyTimes(project);
        return project.Tasks.Max(t => t.EarlyFinish);
    }

    public GetProjectDTO? GetProjectWithCriticalPath(int projectId)
    {
        var project = GetProjectById(projectId);
        if (project == null) return null;

        _criticalPathService.CalculateLateTimes(project);
        var criticalTasks = _criticalPathService.GetCriticalPath(project);
        var estimatedFinish = GetEstimatedProjectFinishDate(project);

        return new GetProjectDTO
        {
            Name = project.Name,
            EstimatedFinish = estimatedFinish,
            CriticalPathTitles = criticalTasks.Select(t => t.Title).ToList(),
            Tasks = project.Tasks.Select(t => new GetTaskDTO
            {
                Title = t.Title,
                Duration = t.Duration,
                Status = t.Status.ToString(),
                EarlyStart = t.EarlyStart,
                EarlyFinish = t.EarlyFinish,
                LateStart = t.LateStart,
                LateFinish = t.LateFinish,
                DateCompleated = t.DateCompleated
            }).ToList(),
            StartDate = project.StartDate
        };
    }

    internal Project? GetProjectById(int projectId)
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

        CalculateTaskDates(taskInRepository, project);

        _projectRepository.Update(project);
        _taskRepository.Update(taskInRepository);
    }
    
    private void CalculateTaskDates(Task task, Project project)
    {
        DateTime earlyStart;

        if (task.Dependencies == null || task.Dependencies.Count == 0)
        {
            earlyStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));
        }
        else
        {
            DateTime latestDependencyFinish = DateTime.MinValue;
        
            foreach (var dependency in task.Dependencies)
            {
                if (dependency.EarlyFinish > latestDependencyFinish)
                {
                    latestDependencyFinish = dependency.EarlyFinish;
                }
            }
        
            earlyStart = latestDependencyFinish != DateTime.MinValue 
                ? latestDependencyFinish.AddDays(1) 
                : project.StartDate.ToDateTime(new TimeOnly(0, 0));
        }

        task.EarlyStart = earlyStart;
        task.EarlyFinish = earlyStart.AddDays(task.Duration);
    }
    
    public string? GetAdminEmailByTaskTitle(string title)
    {
        Project? projectWithTask = _projectRepository.Find(p => p.Tasks.Any(t => t.Title == title));
        return projectWithTask?.Administrator.Email;
    }

    public bool IsExclusiveResourceForProject(int resourceId, int projectId)
    {
        return _projectRepository.FindAll().Where(p => p.Id == projectId)
            .Any(p => p.ExclusiveResources.Any(r => r.Id == resourceId));
    }
    
    public List<User> GetUsersFromProject(int projectId)
    {
        var project = GetProjectById(projectId);
        if (project == null || project.Users == null)
            return new List<User>();
        return project.Users;
    }

    public User? GetAdministratorByProjectId(int projectId)
    {
        Project? project = _projectRepository.Find(p => p.Id == projectId); 
        return project?.ProjectRoles?.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin)?.User;
    }
}