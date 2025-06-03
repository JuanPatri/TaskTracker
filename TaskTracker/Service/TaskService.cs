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
    
    public TaskService(IRepository<Task> taskRepository,
        IRepository<Resource> resourceRepository, IRepository<Project> projectRepository)
    {
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
        _projectRepository = projectRepository;
    }
    
    public Task AddTask(TaskDataDTO taskDto)
    {
        if (_taskRepository.Find(t => t.Title == taskDto.Title) != null)
        {
            throw new Exception("Task already exists");
        }

        List<Task> dependencies = GetTaskDependenciesWithTitleTask(taskDto.Dependencies);

        List<TaskResource> taskResourceList = GetTaskResourcesWithDto(taskDto.Resources);

        Task createdTask = Task.FromDto(taskDto, taskResourceList, dependencies);

        return _taskRepository.Add(createdTask);
    }
    private List<TaskResource> GetTaskResourcesWithDto(List<TaskResourceDataDTO> taskResourceData)
    {
        List<TaskResource> taskResources = new List<TaskResource>();

        if (taskResourceData != null)
        {
            foreach (TaskResourceDataDTO resourceData in taskResourceData)
            {
                Resource resource = _resourceRepository.Find(r => r.Id == resourceData.ResourceId);

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

        return _taskRepository.Update(Task.FromDto(taskDto, taskResourceList, dependencies));
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

        List<Task> dependencies = GetTaskDependenciesWithTitleTask(taskDto.Dependencies ?? new List<string>());
        List<TaskResource> taskResourceList = GetTaskResourcesWithDto(taskDto.Resources ?? new List<TaskResourceDataDTO>());
        Task createdTask = Task.FromDto(taskDto, taskResourceList, dependencies);    
    
        DateTime earlyStart;
    
        if (createdTask.Dependencies == null || createdTask.Dependencies.Count == 0)
        {
            earlyStart = project.StartDate.ToDateTime(new TimeOnly(0, 0));
        }
        else
        {
            earlyStart = createdTask.Dependencies.Max(dep => dep.EarlyFinish);
        }
    
        DateTime earlyFinish = earlyStart.AddDays(createdTask.Duration);
    
        return (earlyStart, earlyFinish);
    }
    
    public List<Task> GetTaskDependenciesWithTitleTask(List<string> titlesTask)
    {
        return _taskRepository.FindAll()
            .Where(t => titlesTask.Contains(t.Title))
            .ToList();
    }


}