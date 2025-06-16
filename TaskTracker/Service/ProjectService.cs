using Domain;
using DTOs.ExporterDTOs;
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
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private readonly IRepository<User> _userRepository;
    public ProjectDataDTO? SelectedProject;
    private readonly UserService _userService;
    private readonly CriticalPathService _criticalPathService;
    private int _idResource;

    public ProjectService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<ResourceType> resourceTypeRepository, IRepository<User> userRepository, UserService userService,
        CriticalPathService criticalPathService)
    {
        _projectRepository = projectRepository;
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

        string adminEmail = project.Users[0];
        string leaderEmail = project.Users[1];

        List<ProjectRole> projectRoles = new List<ProjectRole>();

        foreach (User user in associatedUsers)
        {
            if (user.Email == adminEmail)
            {
                ProjectRole adminRole = new ProjectRole
                {
                    RoleType = RoleType.ProjectAdmin,
                    User = user
                };
                projectRoles.Add(adminRole);
            }
   
            if (user.Email == leaderEmail)
            {
                ProjectRole leaderRole = new ProjectRole
                {
                    RoleType = RoleType.ProjectLead,
                    User = user
                };
                projectRoles.Add(leaderRole);
            }
       
            if (user.Email != adminEmail && user.Email != leaderEmail)
            {
                ProjectRole memberRole = new ProjectRole
                {
                    RoleType = RoleType.ProjectMember,
                    User = user
                };
                projectRoles.Add(memberRole);
            }
        }

        Project newProject = FromDto(project, projectRoles);
        
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

        Project? updatedProject = _projectRepository.Update(FromDto(projectDto, projectRoles));

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
            .Where(ProjectHasUserEditRights(userEmail));

        return filteredProjects.Select(ToGetProjectDTO).ToList();
    }

    private Func<Project, bool> ProjectHasUserEditRights(string userEmail) =>
        project =>
            project.ProjectRoles?.Any(pr =>
                (pr.RoleType == RoleType.ProjectAdmin || pr.RoleType == RoleType.ProjectLead)
                && pr.User.Email == userEmail) == true;

    public List<GetProjectDTO> GetAllProjectsByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(project =>
                project.ProjectRoles != null &&
                project.ProjectRoles.Any(pr => pr.User.Email == userEmail));

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
        
        if (project.ExclusiveResources == null)
            project.ExclusiveResources = new List<Resource>();

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
        
        if (resources == null || !resources.Any())
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
                Users = p.ProjectRoles?.Select(pr => pr.User.Email).ToList() ?? new List<string>()
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

    public  List<ProjectDataDTO> ProjectsDataByUserEmail(string userEmail)
    {
        var filteredProjects = _projectRepository.FindAll()
            .Where(project => project.ProjectRoles != null &&
                              project.ProjectRoles.Any(pr => pr.User.Email == userEmail));

        return filteredProjects.Select(ToProjectDataDto).ToList();
    }

    private ProjectDataDTO ToProjectDataDto(Project project) => new ProjectDataDTO
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        StartDate = project.StartDate,
        Users = project.ProjectRoles?.Select(pr => pr.User.Email).ToList() ?? new List<string>()
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
                if (dependency.Task.EarlyFinish > latestDependencyFinish)
                {
                    latestDependencyFinish = dependency.Task.EarlyFinish;
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
        return projectWithTask?.ProjectRoles?.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin)?.User?.Email;
    }
    
    public string? GetLeadEmailByTaskTitle(string title)
    {
        Project? projectWithTask = _projectRepository.Find(p => p.Tasks.Any(t => t.Title == title));
        return projectWithTask?.ProjectRoles?.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectLead)?.User?.Email;
    }

    public bool IsExclusiveResourceForProject(int resourceId, int projectId)
    {
        return _projectRepository.FindAll().Where(p => p.Id == projectId)
            .Any(p => p.ExclusiveResources.Any(r => r.Id == resourceId));
    }

    public List<User> GetUsersFromProject(int projectId)
    {
        var project = GetProjectById(projectId);
        if (project == null || project.ProjectRoles == null)
            return new List<User>();
        return project.ProjectRoles.Select(pr => pr.User).ToList();
    }

    public User? GetAdministratorByProjectId(int projectId)
    {
        Project? project = _projectRepository.Find(p => p.Id == projectId);
        return project?.ProjectRoles?.FirstOrDefault(pr => pr.RoleType == RoleType.ProjectAdmin)?.User;
    }

    public bool IsLeadProject(ProjectDataDTO project, string leadEmail)
    {
        Project? existingProject = _projectRepository.Find(p => p.Id == project.Id);
        if (existingProject != null && existingProject.ProjectRoles != null)
        {
            return existingProject.ProjectRoles.Any(pr => pr.RoleType == RoleType.ProjectLead && pr.User.Email == leadEmail);
        }
        
        return false;
    }
    
    public List<Project> GetProjectsLedByUser(string email)
    {
        return _projectRepository.FindAll()
            .Where(p => p.ProjectRoles != null && p.ProjectRoles.Any(pr => pr.RoleType == RoleType.ProjectLead && pr.User.Email == email))
            .ToList();
    }

    public List<ProjectExporterDataDto> MapProjectsToExporterDataDto(List<Project> projects)
    {
        return projects.Select(p => new ProjectExporterDataDto
        {
            Name = p.Name,
            StartDate = p.StartDate,
            Tasks = p.Tasks.Select(t => new TaskExporterDataDto
            {
                Title = t.Title,
                StartDate = t.EarlyStart,
                Duration = t.Duration,
                IsCritical = p.CriticalPath != null && p.CriticalPath.Contains(t) ? "S" : "N",
                Resources = t.Resources?.Select(r => r.Resource.Name).ToList() ?? new List<string>()
            }).ToList()
        }).ToList();
    }
    
    public bool HasProjectStarted(int projectId)
    {
        Project project = GetProjectById(projectId);
        if (project == null)
        {
            throw new Exception($"Project with ID {projectId} not found.");
        }

        DateTime projectStartDate = project.StartDate.ToDateTime(new TimeOnly(0, 0));
        return projectStartDate < DateTime.Today;
    }
    
    public Project FromDto(ProjectDataDTO projectDataDto, List<ProjectRole> users)
    {
        return new Project()
        {
            Name = projectDataDto.Name,
            Description = projectDataDto.Description,
            StartDate = projectDataDto.StartDate,
            ProjectRoles = users
        };
    }
}