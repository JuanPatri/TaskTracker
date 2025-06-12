using Domain;
using DTOs.ResourceDTOs;
using DTOs.TaskDTOs;
using DTOs.TaskResourceDTOs;
using Enums;
using Repository;
using Service;
using Task = Domain.Task;


namespace Service;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly TaskService _taskService;
    private readonly ProjectService _projectService;

    public ResourceService(IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository,
        IRepository<Project> projectRepository, TaskService taskService, ProjectService projectService)
    {
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _projectRepository = projectRepository;
        _taskService = taskService;
        _projectService = projectService;
    }

    public Resource? AddResource(ResourceDataDto resource)
    {
        if (_resourceRepository.Find(r => r.Name == resource.Name) != null)
        {
            throw new Exception("Resource already exists");
        }

        if (resource.Quantity <= 0)
        {
            throw new ArgumentException("Resource quantity must be greater than zero");
        }

        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resource.TypeResource);
        
        Resource newResource = FromDto(resource, resourceType);
    
        newResource.Id = GetNextResourceId();

        Resource? createdResource = _resourceRepository.Add(newResource);
        return createdResource;
    }
    
    private int GetNextResourceId()
    {
        var allResources = _resourceRepository.FindAll();
        int maxId = allResources.Any() ? allResources.Max(r => r.Id) : 0;
    
        int nextId = maxId + 1;
        if (nextId >= 1000)
        {
            throw new InvalidOperationException("Too many system resources. Max 999 allowed.");
        }
    
        return nextId;
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
        Resource? updatedResource = _resourceRepository.Update(FromDto(resourceDto, resourceType));
        return updatedResource;
    }

    public List<GetResourceDto> GetResourcesForSystem()
    {
        return _resourceRepository.FindAll()
            .Select(resource => new GetResourceDto
            {
                ResourceId = resource.Id,
                Name = resource.Name
            })
            .ToList();
    }

    public bool IsResourceAvailable(int resourceId, int projectId, bool isExclusive, DateTime taskEarlyStart,
        DateTime taskEarlyFinish, int requiredQuantity, List<TaskResourceDataDTO> pendingTaskResources)
    {
        Resource? resource = null;
        List<Task> tasksToCheck;

        if (isExclusive)
        {
            Project? currentProject = _projectRepository.Find(p => p.Id == projectId);

            resource = currentProject?.ExclusiveResources?.FirstOrDefault(r => r.Id == resourceId);
            tasksToCheck = currentProject?.Tasks
                .Where(task => task.Resources != null &&
                               task.Resources.Any(tr => tr.Resource.Id == resourceId))
                .ToList() ?? new List<Task>();
        }
        else
        {
            resource = _resourceRepository.Find(r => r.Id == resourceId);

            tasksToCheck = _projectRepository.FindAll()
                .SelectMany(p => p.Tasks ?? new List<Task>())
                .Where(task => task.Resources != null && task.Resources.Any(tr => tr.Resource.Id == resourceId))
                .ToList();
        }

        if (resource == null) return false;

        List<Task> overlappingTasks = tasksToCheck
            .Where(task =>
                task.Status != Status.Completed &&
                _taskService.TasksOverlapAtLeastOneDay(task, taskEarlyStart, taskEarlyFinish))
            .ToList();

        int usedQuantity = overlappingTasks
            .SelectMany(task => task.Resources)
            .Where(tr => tr.Resource.Id == resourceId)
            .Sum(tr => tr.Quantity);

        int pendingUsage = pendingTaskResources
            .Where(ptr => ptr.ResourceId == resourceId)
            .Sum(ptr => ptr.Quantity);

        return (resource.Quantity - usedQuantity - pendingUsage) >= requiredQuantity;
    }

    public bool IsResourceAvailableForNewTask(int resourceId, int projectId, bool isExclusive,
        DateTime taskEarlyStart, DateTime taskEarlyFinish, int requiredQuantity)
    {
        Resource? resource = null;
        List<Task> tasksToCheck = new List<Task>();

        if (isExclusive)
        {
            Project? currentProject = _projectRepository.Find(p => p.Id == projectId);
            if (currentProject == null)
            {
                return false;
            }

            resource = currentProject.ExclusiveResources?.FirstOrDefault(r => r.Id == resourceId);

            tasksToCheck = currentProject.Tasks?.ToList() ?? new List<Task>();

            var tasksUsingResource = tasksToCheck.Where(task =>
                task.Resources != null && task.Resources.Any(tr => tr.Resource.Id == resourceId)).ToList();

            tasksToCheck = tasksUsingResource;
        }
        else
        {
            resource = _resourceRepository.Find(r => r.Id == resourceId);

            tasksToCheck = _projectRepository.FindAll()
                .SelectMany(p => p.Tasks ?? new List<Task>())
                .Where(task => task.Resources != null && task.Resources.Any(tr => tr.Resource.Id == resourceId))
                .ToList();
        }

        if (resource == null)
        {
            return false;
        }

        var overlappingTasks = new List<Task>();
        
        foreach (var task in tasksToCheck)
        {
            if (task.Status == Status.Completed)
            {
                continue;
            }

            DateTime taskStart = task.EarlyStart.Date;
            DateTime taskEnd = task.EarlyFinish.Date;
            DateTime newTaskStart = taskEarlyStart.Date;
            DateTime newTaskEnd = taskEarlyFinish.Date;

            bool overlaps = taskStart <= newTaskEnd && newTaskStart <= taskEnd;
            
            if (overlaps)
            {
                overlappingTasks.Add(task);
            }
        }

        int totalUsedQuantity = 0;
        foreach (var task in overlappingTasks)
        {
            var resourcesUsed = task.Resources.Where(tr => tr.Resource.Id == resourceId).ToList();
            foreach (var tr in resourcesUsed)
            {
                totalUsedQuantity += tr.Quantity;
            }
        }

        int availableQuantity = resource.Quantity - totalUsedQuantity;
        
        return availableQuantity >= requiredQuantity;
    }
    
    public void DecreaseResourceQuantity(int projectId, string resourceName)
    {
        Project project = _projectRepository.Find(p => p.Id == projectId);
        if (project == null)
            throw new ArgumentException("Project not found");

        bool updated = false;

        foreach (var task in project.Tasks)
        {
            foreach (var taskResource in task.Resources)
            {
                if (taskResource.Resource.Name == resourceName && taskResource.Quantity > 0)
                {
                    taskResource.Quantity -= 1;
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
    
    public Resource FromDto(ResourceDataDto resourceDataDto,ResourceType resourceType)
    {
        return new Resource()
        {
            Name = resourceDataDto.Name,
            Description = resourceDataDto.Description,
            Type = resourceType,
            Quantity = resourceDataDto.Quantity
        };
    }


    public ResourceConflictDto CheckAndResolveConflicts(TaskDataDTO taskDto, int projectId, bool autoResolve = false)
    {
        ResourceConflictDto result = new ResourceConflictDto();

        if (taskDto.Resources?.Any() != true)
            return result;

        (DateTime taskStart, DateTime taskEnd) dates = _taskService.GetTaskDatesFromDto(taskDto, projectId);
        List<string> allConflictingTasks = new List<string>();
        List<string> conflictMessages = new List<string>();

        foreach (TaskResourceDataDTO resource in taskDto.Resources)
        {
            bool isExclusive = _projectService.IsExclusiveResourceForProject(resource.ResourceId, projectId);

            bool isAvailable = IsResourceAvailable(
                resource.ResourceId,
                projectId,
                isExclusive,
                dates.taskStart,
                dates.taskEnd,
                resource.Quantity,
                new List<TaskResourceDataDTO>()
            );

            if (!isAvailable)
            {
                List<string> conflictingTasks = FindConflictingTasksForResource(
                    resource.ResourceId, projectId, dates.taskStart, dates.taskEnd);

                allConflictingTasks.AddRange(conflictingTasks);

                Resource resourceInfo = GetResourceInfo(resource.ResourceId, projectId);
                string resourceName = resourceInfo?.Name ?? "Unknown Resource";
                conflictMessages.Add($"{resourceName} conflicts with: {string.Join(", ", conflictingTasks)}");
            }
        }

        if (allConflictingTasks.Any())
        {
            result.HasConflicts = true;
            result.ConflictingTasks = allConflictingTasks.Distinct().ToList();
            result.Message = string.Join("; ", conflictMessages);

            if (autoResolve)
            {
                foreach (string conflictingTask in result.ConflictingTasks)
                {
                    if (!taskDto.Dependencies.Contains(conflictingTask))
                    {
                        taskDto.Dependencies.Add(conflictingTask);
                    }
                }
            }
        }

        return result;
    }


    private List<string> FindConflictingTasksForResource(int resourceId, int projectId, DateTime taskStart,
        DateTime taskEnd)
    {
        bool isExclusive = _projectService.IsExclusiveResourceForProject(resourceId, projectId);
        List<Task> allTasks;

        if (isExclusive)
        {
            Project project = _projectService.GetProjectById(projectId);
            allTasks = project?.Tasks?.ToList() ?? new List<Task>();
        }
        else
        {
            allTasks = _projectRepository.FindAll().SelectMany(p => p.Tasks ?? new List<Task>()).ToList();
        }

        List<string> conflictingTaskTitles = allTasks
            .Where(task =>
                task.Resources?.Any(tr => tr.Resource.Id == resourceId) == true &&
                task.Status != Status.Completed &&
                task.EarlyStart.Date <= taskEnd.Date &&
                taskStart.Date <= task.EarlyFinish.Date)
            .Select(task => task.Title)
            .ToList();

        return conflictingTaskTitles;
    }

    private Resource GetResourceInfo(int resourceId, int projectId)
    {
        bool isExclusive = _projectService.IsExclusiveResourceForProject(resourceId, projectId);

        if (isExclusive)
        {
            Project project = _projectService.GetProjectById(projectId);
            return project?.ExclusiveResources?.FirstOrDefault(r => r.Id == resourceId);
        }
        else
        {
            return GetAllResources().FirstOrDefault(r => r.Id == resourceId);
        }
    }

    public List<ResourceStatsDto> GetResourceStatsByProject(int idProject)
    {
        Project? project = _projectRepository.FindAll()
            .FirstOrDefault(p => p.Id == idProject);

        List<ResourceStatsDto> resourceStats = new List<ResourceStatsDto>();

        foreach (var task in project.Tasks)
        {

            foreach (var resource in task.Resources)
            {
                ResourceStatsDto resourceStat = new ResourceStatsDto
                {
                    Name = resource.Resource.Name,
                    Description = resource.Resource.Description,
                    Type = resource.Resource.Type.Name,
                    Quantity = resource.Resource.Quantity,
                    TaskName = resource.Task.Title,
                    UsageLevel = task.Resources.Count,
                    UsagePeriod = $"{task.EarlyStart:yyyy-MM-dd} - {task.EarlyFinish:yyyy-MM-dd}"
                };

                resourceStats.Add(resourceStat);
            }
        }

        Console.WriteLine($"✅ Total de recursos agregados: {resourceStats.Count}");

        return resourceStats;
    }
}