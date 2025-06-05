using Domain;
using DTOs.ResourceDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Service;
using Task = Domain.Task;

namespace ServiceTest;

[TestClass]
public class ResourceServiceTest
{
    private IRepository<Resource> _resourceRepository;
    private IRepository<ResourceType> _resourceTypeRepository;
    private ResourceService _resourceService;
    private Resource _resource;
    private IRepository<Project> _projectRepository;
    private TaskService _taskService;
    private TaskRepository _taskRepository;
    private ProjectService _projectService;
    private UserRepository _userRepository;
    private UserService _userService;
    private CriticalPathService _criticalPathService;
    private Project _project;
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    { 
        _resourceRepository = new ResourceRepository();
        _resourceTypeRepository = new ResourceTypeRepository();
        _projectRepository = new ProjectRepository();
        _taskRepository = new TaskRepository();
        _userRepository = new UserRepository();
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository, _userRepository, _userService, _criticalPathService);
        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository, _projectService,
            _criticalPathService);
        _resourceService = new ResourceService(_resourceRepository, _resourceTypeRepository, _projectRepository, _taskService);
        
        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User()
        };
        _projectRepository.Add(_project);
        
        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(_resource);
        _resourceTypeRepository.Add(_resource.Type);
        
        _task = new Task() { Title = "Test Task", };
        _taskRepository.Add(_task);
    }
    
    
    [TestMethod]
    public void CreateResourceService()
    {
        Assert.IsNotNull(_resourceService);
    }

    [TestMethod]
    public void AddResourceShouldReturnResource()
    {
        ResourceDataDto resource = new ResourceDataDto()
        {
            Name = "name",
            Description = "description",
            TypeResource = 1,
            Quantity = 5
        };

        Resource? createdResource = _resourceService.AddResource(resource);
        Assert.IsNotNull(createdResource);
        Assert.AreEqual(_resourceRepository.FindAll().Last(), createdResource);
    }

    [TestMethod]
    public void AddResourceShouldThrowExceptionIfResourceAlreadyExists()
    {
        ResourceDataDto resource = new ResourceDataDto()
        {
            Name = "Resource",
            Description = "description",
            TypeResource = 1
        };

        Assert.ThrowsException<Exception>(() => _resourceService.AddResource(resource));
    }

    [TestMethod]
    public void RemoveResourceShouldRemoveResource()
    {
        Assert.AreEqual(_resourceRepository.FindAll().Count, 1);

        GetResourceDto resourceToDelete = new GetResourceDto()
        {
            Name = "Resource"
        };

        _resourceService.RemoveResource(resourceToDelete);

        Assert.AreEqual(_resourceRepository.FindAll().Count, 0);
    }

    [TestMethod]
    public void GetResourceReturnResource()
    {
        GetResourceDto resourceToFind = new GetResourceDto()
        {
            Name = "Resource"
        };

        Assert.AreEqual(_resourceService.GetResource(resourceToFind), _resource);
    }

    [TestMethod]
    public void GetAllResourceReturnAllResource()
    {
        Resource newResource = new Resource()
        {
            Name = "Resource2",
            Description = "Description",
            Type = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            }
        };
        _resourceRepository.Add(newResource);
        List<Resource> resources = _resourceService.GetAllResources();

        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Resource2"));
    }

    [TestMethod]
    public void UpdateResourceShouldModifyResourceData()
    {
        Assert.AreEqual(_resource.Description, "Description");

        ResourceDataDto resourceDTO = new ResourceDataDto()
        {
            Name = "Resource",
            Description = "new description",
            TypeResource = 2
        };

        _resourceService.UpdateResource(resourceDTO);
        Assert.AreEqual(_resource.Description, "new description");
    }
    
    [TestMethod]
    public void GetResourcesForSystemShouldReturnAllResourceNames()
    {
        Resource additionalResource = new Resource()
        {
            Name = "Additional Resource",
            Description = "Additional description",
            Type = new ResourceType()
            {
                Id = 3,
                Name = "Additional Type"
            }
        };
        _resourceRepository.Add(additionalResource);

        List<GetResourceDto> resources = _resourceService.GetResourcesForSystem();

        Assert.IsNotNull(resources);
        Assert.AreEqual(2, resources.Count);
        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Additional Resource"));
    }
    
    [TestMethod]
    public void GetResourcesWhitNameShouldReturnResources()
    {
        Resource newResource = new Resource();
        newResource.Name = "Resource1";
        _resourceRepository.Add(newResource);
     
        Resource newResource2 = new Resource();
        newResource2.Name = "Resource2";
        _resourceRepository.Add(newResource2);
     
     
        List<(int, string)> searchList = new List<(int, string)>()
        {
            (2, "Resource1")
        };
     
        List<(int, Resource)> resource = _resourceService.GetResourcesWithName(searchList);
     
        Assert.AreEqual(1, resource.Count);
    }
    
    [TestMethod]
    public void DecreaseResourceQuantityShouldThrowWhenResourceQuantityIsZero()
    {
        TaskResource taskResource = new TaskResource()
        {
            Task = new Task(),
            Resource = new Resource(),
            Quantity = 0, 
        };
    
        Task task = new Task
        {
            Title = "Test Task",
            Resources = new List<TaskResource> { taskResource } 
        };
    
        Project project = new Project
        {
            Id = 101,
            Name = "Test Project",
            Tasks = new List<Task> { task },
            Description = "desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User()
        };
    
        _projectRepository.Add(project);
    
        Assert.ThrowsException<InvalidOperationException>(() =>
            _resourceService.DecreaseResourceQuantity(101, "Laptop"));
    }
    
        [TestMethod]
    public void IsExclusiveResourceForProject_ShouldReturnCorrectExclusivity()
    {
        var exclusiveResource = new Resource
        {
            Id = 100,
            Name = "Exclusive Printer",
            Description = "Project exclusive printer",
            Type = _resource.Type
        };

        _project.ExclusiveResources = new List<Resource> { exclusiveResource };

        bool isExclusive1 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, _project.Id);
        Assert.IsTrue(isExclusive1, "Should return true for exclusive resource in the project");

        bool isExclusive2 = _projectService.IsExclusiveResourceForProject(_resource.Id, _project.Id);
        Assert.IsFalse(isExclusive2, "Should return false for non-exclusive resource");

        bool isExclusive3 = _projectService.IsExclusiveResourceForProject(999, _project.Id);
        Assert.IsFalse(isExclusive3, "Should return false for non-existent resource");

        bool isExclusive4 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, 999);
        Assert.IsFalse(isExclusive4, "Should return false for non-existent project");

        var anotherProject = new Project
        {
            Id = 50,
            Name = "Another Project",
            Description = "Different project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User(),
            ExclusiveResources = new List<Resource> { exclusiveResource }
        };
        _projectRepository.Add(anotherProject);

        bool isExclusive5 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, _project.Id);
        Assert.IsTrue(isExclusive5, "Should still return true - resource is in the target project");

        bool isExclusive6 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, anotherProject.Id);
        Assert.IsTrue(isExclusive6, "Should return true for the other project that also has the resource");
    }
    
    [TestMethod]
    public void IsResourceAvailable_ShouldReturnCorrectAvailabilityBasedOnResourceUsage()
    {
        _resource.Quantity = 5;

        var taskStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        var taskEnd = taskStart.AddDays(3);

        _task.EarlyStart = taskStart;
        _task.EarlyFinish = taskEnd;
        _task.Status = Status.Blocked;

        var taskResource = new TaskResource
        {
            Task = _task,
            Resource = _resource,
            Quantity = 2
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _project.Tasks = new List<Task> { _task };

        var newTaskStart = taskStart.AddDays(1);
        var newTaskEnd = newTaskStart.AddDays(2);

        bool isAvailable1 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 2);

        Assert.IsTrue(isAvailable1, "Should be available: 5 total - 2 used = 3 available, requesting 2");

        bool isAvailable2 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 4);

        Assert.IsFalse(isAvailable2, "Should not be available: 5 total - 2 used = 3 available, requesting 4");

        var noOverlapStart = taskEnd.AddDays(1);
        var noOverlapEnd = noOverlapStart.AddDays(2);

        bool isAvailable3 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, noOverlapStart, noOverlapEnd, 5);

        Assert.IsTrue(isAvailable3, "Should be available: no overlap, all 5 units available");

        bool isAvailable4 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, false, newTaskStart, newTaskEnd, 3);

        Assert.IsTrue(isAvailable4, "Should be available: non-exclusive, same calculation");

        _task.Status = Status.Completed;

        bool isAvailable5 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 5);

        Assert.IsTrue(isAvailable5, "Should be available: completed task doesn't count");

        bool isAvailable6 = _resourceService.IsResourceAvailable(
            999, _project.Id, true, newTaskStart, newTaskEnd, 1);

        Assert.IsFalse(isAvailable6, "Should return false for non-existent resource");
    }

    
     [TestMethod]
     public void GetResourcesWithName_ShouldReturnResourceWithCorrectQuantity()
     {
         Resource resource = new Resource { Name = "Laptop" };
         _resourceRepository.Add(resource);
    
         List<(int, string)> resourceList = new List<(int, string)> { (5, "Laptop") };
    
         List<(int, Resource)> result = _resourceService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(1, result.Count);
         Assert.AreEqual(5, result[0].Item1);
         Assert.AreEqual("Laptop", result[0].Item2.Name);
     }

     [TestMethod]
     public void GetResourcesWithName_ShouldIgnoreNonexistentResources()
     {
         var resourceList = new List<(int, string)>
         {
             (2, "NonExistingResource")
         };
    
         var result = _resourceService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(0, result.Count);
     }

    
     [TestMethod]
     public void GetResourcesWithNameShouldMapMultipleResources()
     {
         Resource res1 = new Resource { Name = "Mouse" };
         Resource res2 = new Resource { Name = "Keyboard" };
         _resourceRepository.Add(res1);
         _resourceRepository.Add(res2);
    
         List<(int, string)> resourceList = new List<(int, string)>
         {
             (1, "Mouse"),
             (3, "Keyboard")
         };
    
         List<(int, Resource)> result = _resourceService.GetResourcesWithName(resourceList);
    
         Assert.AreEqual(2, result.Count);
         Assert.IsTrue(result.Any(r => r.Item2.Name == "Mouse" && r.Item1 == 1));
         Assert.IsTrue(result.Any(r => r.Item2.Name == "Keyboard" && r.Item1 == 3));
     }
     
     [TestMethod]
     public void DecreaseResourceQuantityShouldThrowWhenProjectNotFound()
     {
         Assert.ThrowsException<ArgumentException>(() =>
             _resourceService.DecreaseResourceQuantity(999, "x"));
     }
     
     
     [TestMethod]
     public void DecreaseResourceQuantity_ResourceNotFound_ThrowsException()
     {
         Project project = new Project
         {
             Id = 1,
             Tasks = new List<Task>()
         };
         _projectRepository.Add(project);

         var exception = Assert.ThrowsException<InvalidOperationException>(() =>
             _resourceService.DecreaseResourceQuantity(1, "NonExistent"));
    
         Assert.AreEqual("Resource not found or quantity is already 0", exception.Message);
     }

     [TestMethod]
     public void DecreaseResourceQuantity_QuantityIsZero_ThrowsException()
     {
         Resource resource = new Resource { Name = "Printer" };
         TaskResource taskResource = new TaskResource
         {
             Task = new Task(),
             Resource = resource,
             Quantity = 0  
         };

         Task task = new Task
         {
             Title = "Setup",
             Resources = new List<TaskResource> { taskResource }
         };

         Project project = new Project
         {
             Id = 1,
             Tasks = new List<Task> { task }
         };

         _projectRepository.Add(project);

         Assert.ThrowsException<InvalidOperationException>(() =>
             _resourceService.DecreaseResourceQuantity(1, "Printer"));
     }

     [TestMethod]
     public void GetExclusiveResourcesForProjectShouldReturnEmptyWhenProjectNotFound()
     {
         List<GetResourceDto> result = _projectService.GetExclusiveResourcesForProject(999);
         Assert.IsNotNull(result);
         Assert.AreEqual(0, result.Count);
     }
     
     [TestMethod]
     public void GetExclusiveResourcesForProjectShouldReturnExclusiveResources()
     {
         int projectId = 10;

         Resource exclusiveResource1 = new Resource
         {
             Name = "Printer",
             Description = "Laser printer",
             Type = new ResourceType { Id = 1, Name = "Hardware" }
         };

         Resource exclusiveResource2 = new Resource
         {
             Name = "Designer",
             Description = "Graphic Designer",
             Type = new ResourceType { Id = 2, Name = "Human" }
         };

         Project project = new Project
         {
             Id = projectId,
             Name = "Exclusive Project",
             Description = "Description",
             StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
             Administrator = new User { Name = "Admin", Email = "admin@example.com" },
             ExclusiveResources = new List<Resource> { exclusiveResource1, exclusiveResource2 }
         };

         _projectRepository.Add(project);

         List<GetResourceDto> result = _projectService.GetExclusiveResourcesForProject(projectId);

         Assert.IsTrue(result.Any(r => r.Name == "Printer"));
         Assert.IsTrue(result.Any(r => r.Name == "Designer"));
     }
     
     [TestMethod]
     public void AddExclusiveResourceShouldAddCorrectly()
     {
         ResourceDataDto resourceDto = new ResourceDataDto()
             { Name = "Programmer Java", Description = "java", TypeResource = 1 };
         _projectService.AddExclusiveResourceToProject(35, resourceDto);

         Project updatedProject = _projectRepository.Find(p => p.Id == 35);
         Assert.AreEqual(1, updatedProject.ExclusiveResources.Count);
         Assert.AreEqual("Programmer Java", updatedProject.ExclusiveResources[0].Name);
     }

     [TestMethod]
     public void AddExclusiveResourceShouldThrowWhenProjectDoesNotExist()
     {
         ResourceDataDto resourceDto = new ResourceDataDto()
         {
             Name = "Nonexistent Project Resource",
             Description = "Description",
             TypeResource = 1
         };

         Assert.ThrowsException<ArgumentException>(() =>
             _projectService.AddExclusiveResourceToProject(999, resourceDto)
         );
     }
 
     [TestMethod]
     public void GetNextResourceId_NoResources_ReturnsId1()
     {
         var result = _resourceService.AddResource(new ResourceDataDto { Name = "Test", Quantity = 1, TypeResource = 1 });
         Assert.AreEqual(1, result.Id);
     }

     [TestMethod]
     public void GetNextResourceId_WithResources_ReturnsMaxIdPlusOne()
     {
         _resourceRepository.Add(new Resource { Id = 5, Name = "Resource5" });
         _resourceRepository.Add(new Resource { Id = 2, Name = "Resource2" });

         var result = _resourceService.AddResource(new ResourceDataDto { Name = "Test", Quantity = 1, TypeResource = 1 });
         Assert.AreEqual(6, result.Id);
     }

     [TestMethod]
     public void GetNextResourceId_AtLimit999_ThrowsException()
     {
         _resourceRepository.Add(new Resource { Id = 999, Name = "MaxResource" });

         var ex = Assert.ThrowsException<InvalidOperationException>(() => 
             _resourceService.AddResource(new ResourceDataDto { Name = "Test", Quantity = 1, TypeResource = 1 }));
         Assert.AreEqual("Too many system resources. Max 999 allowed.", ex.Message);
     }
}