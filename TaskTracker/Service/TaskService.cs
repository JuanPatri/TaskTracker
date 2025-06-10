using Domain;
using DTOs.TaskDTOs;
using DTOs.TaskResourceDTOs;
using Enums;
using Repository;
using Task = Domain.Task;

namespace Service;

public class TaskService
{
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly ProjectService _projectService;
    private readonly CriticalPathService _criticalPathService;
    
    public TaskService(IRepository<Task> taskRepository,
        IRepository<Resource> resourceRepository, IRepository<Project> projectRepository, ProjectService projectService, CriticalPathService criticalPathService)
    {
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
        _projectRepository = projectRepository;
        _projectService = projectService;
        _criticalPathService = criticalPathService;
    }
    
    public Task AddTask(TaskDataDTO taskDto)
    {
        if (_taskRepository.Find(t => t.Title == taskDto.Title) != null)
        {
            throw new Exception("Task already exists");
        }

        List<Task> dependencies = GetTaskDependenciesWithTitleTask(taskDto.Dependencies);
        List<TaskResource> taskResourceList = GetTaskResourcesWithDto(taskDto.Resources);
        Task createdTask = FromDto(taskDto, taskResourceList, dependencies);
        Task savedTask = _taskRepository.Add(createdTask);
        
        return savedTask;
    }
    
    private List<TaskResource> GetTaskResourcesWithDto(List<TaskResourceDataDTO> taskResourceData)
    {
        List<TaskResource> taskResources = new List<TaskResource>();

        if (taskResourceData != null)
        {
            foreach (TaskResourceDataDTO resourceData in taskResourceData)
            {
                Resource resource = _resourceRepository.Find(r => r.Id == resourceData.ResourceId);
                
                if (resource == null)
                {
                    foreach (var project in _projectRepository.FindAll())
                    {
                        resource = project.ExclusiveResources?.FirstOrDefault(r => r.Id == resourceData.ResourceId);
                        if (resource != null)
                        {
                            break;
                        }
                    }
                }

                if (resource != null)
                {
                    TaskResource taskResource = new TaskResource()
                    {
                        Resource = resource,
                        Quantity = resourceData.Quantity
                    };
            
                    taskResources.Add(taskResource);
                }
            }
        }
        
        return taskResources;
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
        List<TaskResource> taskResourceList = GetTaskResourcesWithDto(taskDto.Resources);

        return _taskRepository.Update(FromDto(taskDto, taskResourceList, dependencies));
    }

    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);

        foreach (var project in _projectRepository.FindAll())
        {
            project.Tasks.RemoveAll(projTask => projTask.Title == task.Title);
        }
    }
    
    public bool ValidateTaskStatus(string title, Status status)
    {
        List<Task> taskDependencies = GetTaskDependenciesWithTitleTask(new List<string> { title });

        return taskDependencies.Count < 0 || status == Status.Pending;
    }

    public bool CanMarkTaskAsCompleted(TaskDataDTO dto)
    {
        var task = _taskRepository.Find(t => t.Title == dto.Title);
        if (task == null) return false;

        return task.Dependencies.All(d => d.Status == Status.Completed);
    }

    public (DateTime EarlyStart, DateTime EarlyFinish) GetTaskDatesFromDto(TaskDataDTO taskDto, int projectId)
    {
        var project = _projectRepository.Find(p => p.Id == projectId);
        if (project == null)
            throw new ArgumentException($"Project with ID {projectId} not found");

        DateTime earlyStart;

        if (taskDto.Dependencies == null || taskDto.Dependencies.Count == 0)
        {
            earlyStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));
        }
        else
        {
            DateTime latestDependencyFinish = DateTime.MinValue;
        
            foreach (string dependencyTitle in taskDto.Dependencies)
            {
                var dependencyTask = project.Tasks?.FirstOrDefault(t => t.Title == dependencyTitle);
                if (dependencyTask != null)
                {
                    if (dependencyTask.EarlyFinish > latestDependencyFinish)
                    {
                        latestDependencyFinish = dependencyTask.EarlyFinish;
                    }
                }
            }
        
            earlyStart = latestDependencyFinish != DateTime.MinValue 
                ? latestDependencyFinish.AddDays(1) 
                : project.StartDate.ToDateTime(new TimeOnly(0, 0));
        }

        DateTime earlyFinish = earlyStart.AddDays(taskDto.Duration);

        return (earlyStart, earlyFinish);
    }
    
    public List<Task> GetTaskDependenciesWithTitleTask(List<string> titlesTask)
    {
        return _taskRepository.FindAll()
            .Where(t => titlesTask.Contains(t.Title))
            .ToList();
    }

    public bool IsTaskCriticalById(int projectId, string taskTitle)
    {
        var project = _projectService.GetProjectById(projectId);
        if (project == null) return false;

        return IsTaskCritical(project, taskTitle);
    }
    
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

        var criticalPath = _criticalPathService.GetCriticalPath(project);
        return criticalPath.Any(task => task.Title.Equals(taskTitle));
    }

    internal bool TasksOverlapAtLeastOneDay(Task existingTask, DateTime newTaskStart, DateTime newTaskEnd)
    {
        return existingTask.EarlyStart.Date <= newTaskEnd.Date &&
               newTaskStart.Date <= existingTask.EarlyFinish.Date;
    }
    
    public List<GetTaskDTO> GetTasksForProjectWithId(int projectId)
    {
        Project? project = _projectService.GetProjectById(projectId);
        if (project == null) return new List<GetTaskDTO>();

        return project.Tasks
            .Select(task => new GetTaskDTO { Title = task.Title })
            .ToList();
    }

    public void RecalculateTaskDates(int projectId)
    {
        var project = _projectRepository.Find(p => p.Id == projectId);
        if (project == null) return;

        foreach (var task in project.Tasks)
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
    
        _projectRepository.Update(project);
    }
    
    public  Task FromDto(TaskDataDTO taskDataDto, List<TaskResource> resources, List<Task> dependencies)
    {
    
        var task = new Task()
        {
            Title = taskDataDto.Title,
            Description = taskDataDto.Description,
            Duration = taskDataDto.Duration,
            Status = taskDataDto.Status,
            Resources = resources ?? new List<TaskResource>(),
            Dependencies = dependencies ?? new List<Task>(),
        };
        
        return task;
    }
}