using Domain;
using DTOs.ResourceDTOs;
using Enums;
using Repository;
using Task = Domain.Task;

namespace Service;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly TaskService _taskService;
    
    public ResourceService(IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository, IRepository<Project> projectRepository, TaskService taskService)
    {
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
        _projectRepository = projectRepository;
        _taskService = taskService;
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

        Resource newResource = Resource.FromDto(resource, resourceType);
    
        int maxId = GetNextResourceId();
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
        Resource? updatedResource = _resourceRepository.Update(Resource.FromDto(resourceDto, resourceType));
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
        DateTime taskEarlyFinish, int requiredQuantity)
    {
        Resource? resource = _resourceRepository.Find(r => r.Id == resourceId);
        if (resource == null) return false;

        List<Task> tasksToCheck;


        if (isExclusive)
        {
            Project? currentProject = _projectRepository.Find(p => p.Id == projectId);
            tasksToCheck = currentProject?.Tasks
                .Where(task => task.Resources != null &&
                               task.Resources.Any(tr => tr.Resource.Id == resourceId))
                .ToList() ?? new List<Task>();
        }
        else
        {
            tasksToCheck = _projectRepository.FindAll()
                .SelectMany(p => p.Tasks ?? new List<Task>())
                .Where(task => task.Resources != null &&
                               task.Resources.Any(tr => tr.Resource.Id == resourceId))
                .ToList();
        }

        List<Task> overlappingTasks = tasksToCheck
            .Where(task =>
                task.Status != Status.Completed &&
                _taskService.TasksOverlapAtLeastOneDay(task, taskEarlyStart, taskEarlyFinish))
            .ToList();

        int usedQuantity = overlappingTasks
            .SelectMany(task => task.Resources)
            .Where(tr => tr.Resource.Id == resourceId)
            .Sum(tr => tr.Quantity);

        return (resource.Quantity - usedQuantity) >= requiredQuantity;
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

}