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

        Resource? createdResource = _resourceRepository.Add(newResource);
        return createdResource;
    }

    public void RemoveResource(GetResourceDto resource)
    {
        Resource? resourceToDelete = _resourceRepository.Find(r => r.Name == resource.Name);

        if (resourceToDelete == null)
        {
            throw new ArgumentException($"Resource '{resource.Name}' not found");
        }
        
        var (isBeingUsed, activeTasks, completedTasks) = GetResourceUsageDetails(resourceToDelete.Id);

        if (isBeingUsed)
        {
            throw new InvalidOperationException(
                $"Cannot delete resource '{resource.Name}' because it is being used by the following active tasks: " +
                string.Join(", ", activeTasks) + ". " +
                "Please remove the resource from these tasks first or complete the tasks.");
        }
        if (completedTasks.Any())
        {
            Console.WriteLine(
                $"Deleting resource '{resource.Name}' and removing its references from {completedTasks.Count} completed tasks.");
        }

        RemoveResourceReferences(resourceToDelete.Id);
        _resourceRepository.Delete(resource.Name);
    }

    private void RemoveResourceReferences(int resourceId)
    {
        var allProjects = _projectRepository.FindAll();

        foreach (var project in allProjects)
        {
            if (project.Tasks != null)
            {
                bool projectModified = false;

                foreach (var task in project.Tasks)
                {
                    if (task.Resources != null)
                    {
                        var resourcesToRemove = task.Resources
                            .Where(tr => tr.Resource.Id == resourceId)
                            .ToList();

                        foreach (var taskResource in resourcesToRemove)
                        {
                            task.Resources.Remove(taskResource);
                            projectModified = true;
                        }
                    }
                }

                if (projectModified)
                {
                    _projectRepository.Update(project);
                }
            }
        }
    }

    private (bool isBeingUsedByActiveTasks, List<string> activeTaskTitles, List<string> completedTaskTitles)
        GetResourceUsageDetails(int resourceId)
    {
        List<string> activeTasks = new();
        List<string> completedTasks = new();

        var allProjects = _projectRepository.FindAll();

        foreach (var project in allProjects)
        {
            if (project.Tasks != null)
            {
                foreach (var task in project.Tasks)
                {
                    if (task.Resources != null && task.Resources.Any(tr => tr.Resource.Id == resourceId))
                    {
                        if (task.Status == Status.Completed)
                        {
                            completedTasks.Add(task.Title);
                        }
                        else
                        {
                            activeTasks.Add(task.Title);
                        }
                    }
                }
            }
        }

        return (activeTasks.Any(), activeTasks, completedTasks);
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

        if (resourceType == null)
        {
            throw new ArgumentException($"Resource type with ID {resourceDto.TypeResource} not found");
        }

        Resource? existingResource = _resourceRepository.Find(r => r.Id == resourceDto.Id);

        if (existingResource == null)
        {
            throw new ArgumentException($"Resource with ID {resourceDto.Id} not found");
        }

        if (resourceDto.Quantity < existingResource.Quantity)
        {
            int totalUsedQuantity = GetTotalUsedQuantityForResource(resourceDto.Id);

            if (resourceDto.Quantity < totalUsedQuantity)
            {
                throw new InvalidOperationException(
                    $"Cannot reduce resource quantity to {resourceDto.Quantity}. " +
                    $"Currently {totalUsedQuantity} units are being used by active tasks. " +
                    $"Minimum allowed quantity: {totalUsedQuantity}");
            }
        }

        Resource updatedResource = new Resource()
        {
            Id = resourceDto.Id,
            Name = resourceDto.Name,
            Description = resourceDto.Description,
            Type = resourceType,
            Quantity = resourceDto.Quantity,
            ProjectId = existingResource.ProjectId
        };

        return _resourceRepository.Update(updatedResource);
    }

    private int GetTotalUsedQuantityForResource(int resourceId)
    {
        int totalUsed = 0;

        var allProjects = _projectRepository.FindAll();

        foreach (var project in allProjects)
        {
            if (project.Tasks != null)
            {
                foreach (var task in project.Tasks)
                {
                    if (task.Status != Status.Completed && task.Resources != null)
                    {
                        var resourceUsage = task.Resources
                            .Where(tr => tr.Resource.Id == resourceId)
                            .Sum(tr => tr.Quantity);

                        totalUsed += resourceUsage;
                    }
                }
            }
        }

        return totalUsed;
    }

    public List<GetResourceDto> GetResourcesForSystem()
    {
        return _resourceRepository.FindAll()
            .Where(resource => resource.ProjectId == null)
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

    public Resource FromDto(ResourceDataDto resourceDataDto, ResourceType resourceType)
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
                    Quantity = resource.Quantity,
                    TaskName = resource.Task.Title,
                    UsagePeriod = $"{task.EarlyStart:yyyy-MM-dd} - {task.EarlyFinish:yyyy-MM-dd}"
                };

                resourceStats.Add(resourceStat);
            }
        }

        return resourceStats;
    }
}