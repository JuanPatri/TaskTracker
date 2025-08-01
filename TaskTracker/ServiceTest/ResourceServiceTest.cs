using Domain;
using DTOs.ProjectDTOs;
using DTOs.ResourceDTOs;
using DTOs.TaskDTOs;
using DTOs.TaskResourceDTOs;
using DTOs.UserDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;
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
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _resourceRepository = new ResourceRepository(_sqlContext);
        _resourceTypeRepository = new ResourceTypeRepository(_sqlContext);
        _projectRepository = new ProjectRepository(_sqlContext);
        _taskRepository = new TaskRepository(_sqlContext);
        _userRepository = new UserRepository(_sqlContext);
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository,
            _userRepository, _userService, _criticalPathService);
        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository, _projectService,
            _criticalPathService);
        _resourceService =
            new ResourceService(_resourceRepository, _resourceTypeRepository, _projectRepository, _taskService,
                _projectService);

        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };
        _projectRepository.Add(_project);

        var existingResourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (existingResourceType == null)
        {
            var resourceType = new ResourceType()
            {
                Id = 4,
                Name = "Type"
            };
            _resourceTypeRepository.Add(resourceType);
            existingResourceType = resourceType;
        }

        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
            Type = existingResourceType,
            Quantity = 10
        };
        _resourceRepository.Add(_resource);

        _task = new Task()
        {
            Title = "Test Task",
            Description = "Test Description",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue,
            LateStart = DateTime.MinValue,
            LateFinish = DateTime.MinValue
        };
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
        ResourceType existingType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (existingType == null)
        {
            existingType = new ResourceType { Id = 4, Name = "Type" };
            _resourceTypeRepository.Add(existingType);
        }

        Resource newResource = new Resource()
        {
            Name = "Resource2",
            Description = "Description",
            Type = existingType,
            Quantity = 5
        };
        _resourceRepository.Add(newResource);

        List<Resource> resources = _resourceService.GetAllResources();

        Assert.IsTrue(resources.Any(r => r.Name == "Resource"));
        Assert.IsTrue(resources.Any(r => r.Name == "Resource2"));
    }

    [TestMethod]
    public void UpdateResourceShouldModifyResourceData()
    {
        Resource? existingResource = _resourceRepository.Find(r => r.Name == "Resource");
        Assert.IsNotNull(existingResource, "El recurso debe existir en la base de datos");

        Assert.AreEqual("Description", existingResource.Description);

        ResourceType? resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        Assert.IsNotNull(resourceType, "ResourceType con ID 4 debe existir");

        Resource resourceToUpdate = new Resource()
        {
            Id = existingResource.Id,
            Name = "Resource",
            Description = "new description",
            Type = resourceType,
            Quantity = existingResource.Quantity
        };

        Resource? updatedResource = _resourceRepository.Update(resourceToUpdate);

        Assert.IsNotNull(updatedResource);
        Assert.AreEqual("new description", updatedResource.Description);

        Resource? resourceFromDb = _resourceRepository.Find(r => r.Name == "Resource");
        Assert.IsNotNull(resourceFromDb);
        Assert.AreEqual("new description", resourceFromDb.Description);
    }

    [TestMethod]
    public void GetResourcesForSystemShouldReturnAllResourceNames()
    {
        ResourceType additionalType = _resourceTypeRepository.Find(rt => rt.Id == 3);
        if (additionalType == null)
        {
            additionalType = new ResourceType()
            {
                Id = 3,
                Name = "Additional Type"
            };
            _resourceTypeRepository.Add(additionalType);
        }

        Resource additionalResource = new Resource()
        {
            Name = "Additional Resource",
            Description = "Additional description",
            Type = additionalType,
            Quantity = 15
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
        ResourceType resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (resourceType == null)
        {
            resourceType = new ResourceType { Id = 4, Name = "Test Type" };
            _resourceTypeRepository.Add(resourceType);
        }

        Resource newResource = new Resource
        {
            Name = "Resource1",
            Description = "Test resource 1 description",
            Type = resourceType,
            Quantity = 10
        };
        _resourceRepository.Add(newResource);

        Resource newResource2 = new Resource
        {
            Name = "Resource2",
            Description = "Test resource 2 description",
            Type = resourceType,
            Quantity = 5
        };
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
        ResourceType testType = new ResourceType { Id = 13, Name = "Test Type" };
        _resourceTypeRepository.Add(testType);

        Resource testResource = new Resource
        {
            Name = "Laptop",
            Description = "Test laptop",
            Type = testType,
            Quantity = 5
        };

        TaskResource taskResource = new TaskResource()
        {
            Resource = testResource,
            Quantity = 0
        };

        Task task = new Task
        {
            Title = "Decrease Resource Task",
            Description = "Test description",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Now,
            EarlyFinish = DateTime.Now.AddDays(1),
            LateStart = DateTime.Now,
            LateFinish = DateTime.Now.AddDays(1),
            Resources = new List<TaskResource> { taskResource }
        };

        taskResource.Task = task;

        Project project = new Project
        {
            Id = 101,
            Name = "Test Project",
            Tasks = new List<Task> { task },
            Description = "desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };

        _projectRepository.Add(project);

        Assert.ThrowsException<InvalidOperationException>(() =>
            _resourceService.DecreaseResourceQuantity(101, "Laptop"));
    }

    [TestMethod]
    public void IsExclusiveResourceForProject_ShouldReturnCorrectExclusivity()
    {
        Project existingProject = _projectRepository.Find(p => p.Id == _project.Id);
        Assert.IsNotNull(existingProject, "Project should exist in database");

        _projectService.AddExclusiveResourceToProject(existingProject.Id, new ResourceDataDto
        {
            Name = "Exclusive Printer",
            Description = "Project exclusive printer",
            Quantity = 1,
            TypeResource = _resource.Type.Id
        });

        Project updatedProject = _projectRepository.Find(p => p.Id == existingProject.Id);
        Assert.IsNotNull(updatedProject.ExclusiveResources);
        Assert.AreEqual(1, updatedProject.ExclusiveResources.Count);

        Resource exclusiveResource = updatedProject.ExclusiveResources.First();

        bool isExclusive1 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, existingProject.Id);
        Assert.IsTrue(isExclusive1, "Should return true for exclusive resource in the project");

        bool isExclusive2 = _projectService.IsExclusiveResourceForProject(_resource.Id, existingProject.Id);
        Assert.IsFalse(isExclusive2, "Should return false for non-exclusive resource");

        bool isExclusive3 = _projectService.IsExclusiveResourceForProject(999, existingProject.Id);
        Assert.IsFalse(isExclusive3, "Should return false for non-existent resource");

        bool isExclusive4 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, 999);
        Assert.IsFalse(isExclusive4, "Should return false for non-existent project");

        Project anotherProject = new Project
        {
            Id = 50,
            Name = "Another Project",
            Description = "Different project",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };
        Project savedAnotherProject = _projectRepository.Add(anotherProject);

        _projectService.AddExclusiveResourceToProject(savedAnotherProject.Id, new ResourceDataDto
        {
            Name = "Another Exclusive Printer",
            Description = "Different project exclusive printer",
            Quantity = 1,
            TypeResource = _resource.Type.Id
        });

        Project updatedAnotherProject = _projectRepository.Find(p => p.Id == savedAnotherProject.Id);
        Resource anotherExclusiveResource = updatedAnotherProject.ExclusiveResources.First();

        bool isExclusive5 = _projectService.IsExclusiveResourceForProject(exclusiveResource.Id, existingProject.Id);
        Assert.IsTrue(isExclusive5, "Should still return true - resource is in the target project");

        bool isExclusive6 =
            _projectService.IsExclusiveResourceForProject(anotherExclusiveResource.Id, savedAnotherProject.Id);
        Assert.IsTrue(isExclusive6, "Should return true for the other project that has its own exclusive resource");
    }

    [TestMethod]
    public void IsResourceAvailable_ShouldReturnCorrectAvailabilityBasedOnResourceUsage()
    {
        _resource.Quantity = 5;
        _resource.Id = 1000;

        _project.ExclusiveResources = new List<Resource> { _resource };

        DateTime taskStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        DateTime taskEnd = taskStart.AddDays(3);

        _task.EarlyStart = taskStart;
        _task.EarlyFinish = taskEnd;
        _task.Status = Status.Blocked;

        TaskResource taskResource = new TaskResource
        {
            Task = _task,
            Resource = _resource,
            Quantity = 2
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _project.Tasks = new List<Task> { _task };

        DateTime newTaskStart = taskStart.AddDays(1);
        DateTime newTaskEnd = newTaskStart.AddDays(2);

        bool isAvailable1 = _resourceService.IsResourceAvailableForNewTask(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 2);

        Assert.IsTrue(isAvailable1, "Should be available: 5 total - 2 used = 3 available, requesting 2");

        bool isAvailable2 = _resourceService.IsResourceAvailableForNewTask(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 4);

        Assert.IsFalse(isAvailable2, "Should not be available: 5 total - 2 used = 3 available, requesting 4");

        DateTime noOverlapStart = taskEnd.AddDays(1);
        DateTime noOverlapEnd = noOverlapStart.AddDays(2);

        bool isAvailable3 = _resourceService.IsResourceAvailableForNewTask(
            _resource.Id, _project.Id, true, noOverlapStart, noOverlapEnd, 5);

        Assert.IsTrue(isAvailable3, "Should be available: no overlap, all 5 units available");

        bool isAvailable4 = _resourceService.IsResourceAvailableForNewTask(
            _resource.Id, _project.Id, false, newTaskStart, newTaskEnd, 3);

        Assert.IsTrue(isAvailable4, "Should be available: non-exclusive, same calculation");

        _task.Status = Status.Completed;

        bool isAvailable5 = _resourceService.IsResourceAvailableForNewTask(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 5);

        Assert.IsTrue(isAvailable5, "Should be available: completed task doesn't count");

        bool isAvailable6 = _resourceService.IsResourceAvailableForNewTask(
            999, _project.Id, true, newTaskStart, newTaskEnd, 1);

        Assert.IsFalse(isAvailable6, "Should return false for non-existent resource");
    }

    [TestMethod]
    public void IsResourceAvailable_WithPendingResources_ShouldReturnCorrectAvailability()
    {
        _resource.Quantity = 5;
        _resource.Id = 1000;

        _project.ExclusiveResources = new List<Resource> { _resource };

        DateTime taskStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        DateTime taskEnd = taskStart.AddDays(3);

        _task.EarlyStart = taskStart;
        _task.EarlyFinish = taskEnd;
        _task.Status = Status.Blocked;

        TaskResource taskResource = new TaskResource
        {
            Task = _task,
            Resource = _resource,
            Quantity = 2
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _project.Tasks = new List<Task> { _task };

        DateTime newTaskStart = taskStart.AddDays(1);
        DateTime newTaskEnd = newTaskStart.AddDays(2);

        List<TaskResourceDataDTO> pendingResources = new List<TaskResourceDataDTO>
        {
            new TaskResourceDataDTO
            {
                ResourceId = _resource.Id,
                Quantity = 1
            }
        };

        bool isAvailable1 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 2, pendingResources);

        Assert.IsTrue(isAvailable1, "Should be available: 5 total - 2 used - 1 pending = 2 available, requesting 2");

        bool isAvailable2 = _resourceService.IsResourceAvailable(
            _resource.Id, _project.Id, true, newTaskStart, newTaskEnd, 3, pendingResources);

        Assert.IsFalse(isAvailable2,
            "Should not be available: 5 total - 2 used - 1 pending = 2 available, requesting 3");
    }

    [TestMethod]
    public void GetResourcesWithName_ShouldReturnResourceWithCorrectQuantity()
    {
        ResourceType resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (resourceType == null)
        {
            resourceType = new ResourceType { Id = 4, Name = "Test Type" };
            _resourceTypeRepository.Add(resourceType);
        }

        Resource resource = new Resource
        {
            Name = "Laptop",
            Description = "Test laptop description",
            Type = resourceType,
            Quantity = 10
        };
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
        List<(int, string)> resourceList = new List<(int, string)>
        {
            (2, "NonExistingResource")
        };

        List<(int, Resource)> result = _resourceService.GetResourcesWithName(resourceList);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetResourcesWithNameShouldMapMultipleResources()
    {
        ResourceType resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (resourceType == null)
        {
            resourceType = new ResourceType { Id = 4, Name = "Test Type" };
            _resourceTypeRepository.Add(resourceType);
        }

        Resource res1 = new Resource
        {
            Name = "Mouse",
            Description = "Test mouse description",
            Type = resourceType,
            Quantity = 5
        };

        Resource res2 = new Resource
        {
            Name = "Keyboard",
            Description = "Test keyboard description",
            Type = resourceType,
            Quantity = 3
        };

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
            Name = "Empty Project",
            Description = "Project with no resources",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task>()
        };
        _projectRepository.Add(project);

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(() =>
            _resourceService.DecreaseResourceQuantity(1, "NonExistent"));

        Assert.AreEqual("Resource not found or quantity is already 0", exception.Message);
    }

    [TestMethod]
    public void DecreaseResourceQuantity_QuantityIsZero_ThrowsException()
    {
        ResourceType testType = new ResourceType { Id = 14, Name = "Test Type" };
        _resourceTypeRepository.Add(testType);

        Resource resource = new Resource
        {
            Name = "Printer",
            Description = "Test printer",
            Type = testType,
            Quantity = 10
        };

        Task innerTask = new Task
        {
            Title = "Inner Task",
            Description = "Inner task description",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Now,
            EarlyFinish = DateTime.Now.AddDays(1),
            LateStart = DateTime.Now,
            LateFinish = DateTime.Now.AddDays(1)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = innerTask,
            Resource = resource,
            Quantity = 0
        };

        Task task = new Task
        {
            Title = "Setup",
            Description = "Setup task description",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Now,
            EarlyFinish = DateTime.Now.AddDays(1),
            LateStart = DateTime.Now,
            LateFinish = DateTime.Now.AddDays(1),
            Resources = new List<TaskResource> { taskResource }
        };

        Project project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "Test project description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>(),
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

        ResourceType hardwareType = new ResourceType { Id = 10, Name = "Hardware" };
        ResourceType humanType = new ResourceType { Id = 11, Name = "Human" };
        _resourceTypeRepository.Add(hardwareType);
        _resourceTypeRepository.Add(humanType);

        Resource exclusiveResource1 = new Resource
        {
            Name = "Printer",
            Description = "Laser printer",
            Type = hardwareType,
            Quantity = 1
        };

        Resource exclusiveResource2 = new Resource
        {
            Name = "Designer",
            Description = "Graphic Designer",
            Type = humanType,
            Quantity = 1
        };

        Project project = new Project
        {
            Id = projectId,
            Name = "Exclusive Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>(),
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
    public void GetNextResourceId_NoResources_ReturnsNextAvailableId()
    {
        var existingResources = _resourceRepository.FindAll();
        foreach (var resource in existingResources)
        {
            _resourceRepository.Delete(resource.Name);
        }

        ResourceType resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (resourceType == null)
        {
            resourceType = new ResourceType { Id = 4, Name = "Test Type" };
            _resourceTypeRepository.Add(resourceType);
        }

        Resource? result = _resourceService.AddResource(new ResourceDataDto
        {
            Name = "Test",
            Quantity = 1,
            TypeResource = 4,
            Description = "Test description"
        });

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Id > 0);
        Assert.AreEqual("Test", result.Name);
    }

    [TestMethod]
    public void GetNextResourceId_WithResources_ReturnsMaxIdPlusOne()
    {
        ResourceType resourceType = _resourceTypeRepository.Find(rt => rt.Id == 4);
        if (resourceType == null)
        {
            resourceType = new ResourceType { Id = 4, Name = "Test Type" };
            _resourceTypeRepository.Add(resourceType);
        }

        _resourceRepository.Add(new Resource
        {
            Id = 5,
            Name = "Resource5",
            Description = "Test resource 5 description",
            Type = resourceType,
            Quantity = 10
        });

        _resourceRepository.Add(new Resource
        {
            Id = 2,
            Name = "Resource2",
            Description = "Test resource 2 description",
            Type = resourceType,
            Quantity = 5
        });

        Resource? result = _resourceService.AddResource(new ResourceDataDto
        {
            Name = "Test",
            Quantity = 1,
            TypeResource = 4,
            Description = "Test description"
        });

        Assert.AreEqual(6, result.Id);
    }

    [TestMethod]
    public void AddResource_ValidationTests()
    {
        ResourceDataDto zeroQuantityResource = new ResourceDataDto()
        {
            Name = "ZeroResource",
            Description = "description",
            TypeResource = 1,
            Quantity = 0
        };
        ArgumentException exception1 =
            Assert.ThrowsException<ArgumentException>(() => _resourceService.AddResource(zeroQuantityResource));
        Assert.AreEqual("Resource quantity must be greater than zero", exception1.Message);

        ResourceDataDto negativeResource = new ResourceDataDto()
        {
            Name = "NegativeResource",
            Description = "description",
            TypeResource = 1,
            Quantity = -5
        };
        ArgumentException exception2 =
            Assert.ThrowsException<ArgumentException>(() => _resourceService.AddResource(negativeResource));
        Assert.AreEqual("Resource quantity must be greater than zero", exception2.Message);
    }

    [TestMethod]
    public void IsResourceAvailable_ErrorCases()
    {
        List<TaskResourceDataDTO> pendingResources = new List<TaskResourceDataDTO>();
        DateTime today = DateTime.Now;
        DateTime tomorrow = today.AddDays(1);

        bool result1 =
            _resourceService.IsResourceAvailable(_resource.Id, 999, true, today, tomorrow, 1, pendingResources);
        Assert.IsFalse(result1);

        bool result2 =
            _resourceService.IsResourceAvailable(999, _project.Id, true, today, tomorrow, 1, pendingResources);
        Assert.IsFalse(result2);

        bool result3 = _resourceService.IsResourceAvailableForNewTask(_resource.Id, 999, true, today, tomorrow, 1);
        Assert.IsFalse(result3);

        bool result4 = _resourceService.IsResourceAvailableForNewTask(999, _project.Id, true, today, tomorrow, 1);
        Assert.IsFalse(result4);
    }

    public void DecreaseResourceQuantity_AndEdgeCases()
    {
        Resource resource = new Resource { Name = "TestResource", Quantity = 5 };
        TaskResource taskResource = new TaskResource
        {
            Task = new Task(),
            Resource = resource,
            Quantity = 3
        };
        Task task = new Task
        {
            Title = "Test Task",
            Resources = new List<TaskResource> { taskResource }
        };
        Project project = new Project
        {
            Id = 200,
            Name = "Test Project",
            Tasks = new List<Task> { task },
            Description = "desc",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };
        _projectRepository.Add(project);

        _resourceService.DecreaseResourceQuantity(200, "TestResource");
        Assert.AreEqual(2, taskResource.Quantity);

        List<(int, Resource)> emptyResult = _resourceService.GetResourcesWithName(new List<(int, string)>());
        Assert.AreEqual(0, emptyResult.Count);

        Resource newRes = new Resource { Name = "MixedTest" };
        _resourceRepository.Add(newRes);
        List<(int, string)> mixedList = new List<(int, string)> { (1, "MixedTest"), (2, "NonExistent") };
        List<(int, Resource)> mixedResult = _resourceService.GetResourcesWithName(mixedList);
        Assert.AreEqual(1, mixedResult.Count);
        Assert.AreEqual("MixedTest", mixedResult[0].Item2.Name);
    }

    [TestMethod]
    public void CheckAndResolveConflicts_NoResourcesOrNoConflicts_ReturnsNoConflicts()
    {
        TaskDataDTO taskWithoutResources = new TaskDataDTO
        {
            Title = "No Resources Task",
            Description = "Task without resources",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO>()
        };

        ResourceConflictDto result1 =
            _resourceService.CheckAndResolveConflicts(taskWithoutResources, _project.Id, false);
        Assert.IsFalse(result1.HasConflicts);
        Assert.AreEqual(0, result1.ConflictingTasks.Count);

        TaskResourceDataDTO taskResource = new TaskResourceDataDTO
        {
            TaskTitle = "No Conflict Task",
            ResourceId = _resource.Id,
            Quantity = 1
        };

        TaskDataDTO taskWithResources = new TaskDataDTO
        {
            Title = "No Conflict Task",
            Description = "Task with sufficient resources",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { taskResource }
        };

        ResourceConflictDto result2 = _resourceService.CheckAndResolveConflicts(taskWithResources, _project.Id, false);
        Assert.IsFalse(result2.HasConflicts);

        TaskResourceDataDTO nonExistentResource = new TaskResourceDataDTO
        {
            TaskTitle = "Non Existent Resource Task",
            ResourceId = 999,
            Quantity = 1
        };

        TaskDataDTO taskWithNonExistentResource = new TaskDataDTO
        {
            Title = "Non Existent Resource Task",
            Description = "Task with non-existent resource",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { nonExistentResource }
        };

        ResourceConflictDto result3 =
            _resourceService.CheckAndResolveConflicts(taskWithNonExistentResource, _project.Id, false);
        Assert.IsFalse(result3.HasConflicts);
    }

    [TestMethod]
    public void CheckAndResolveConflicts_HasConflicts_DetectsAndOptionallyResolves()
    {
        ResourceType limitedType = new ResourceType { Id = 9, Name = "Limited Type" };
        _resourceTypeRepository.Add(limitedType);

        Resource limitedResource = new Resource
        {
            Name = "Limited Resource",
            Description = "Resource with limited quantity",
            Type = limitedType,
            Quantity = 1
        };
        _resourceRepository.Add(limitedResource);

        TaskResource taskResource = new TaskResource
        {
            Resource = limitedResource,
            Quantity = 1
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _task.EarlyStart = DateTime.Now.AddDays(1);
        _task.EarlyFinish = DateTime.Now.AddDays(5);
        _task.Status = Status.Pending;
        _project.Tasks.Add(_task);

        TaskResourceDataDTO conflictingResource = new TaskResourceDataDTO
        {
            TaskTitle = "Conflicting Task",
            ResourceId = limitedResource.Id,
            Quantity = 1
        };

        TaskDataDTO conflictingTask = new TaskDataDTO
        {
            Title = "Conflicting Task",
            Description = "Task that conflicts with existing task",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { conflictingResource }
        };

        ResourceConflictDto result1 =
            _resourceService.CheckAndResolveConflicts(conflictingTask, _project.Id, autoResolve: false);
        Assert.IsTrue(result1.HasConflicts);
        Assert.AreEqual(1, result1.ConflictingTasks.Count);
        Assert.AreEqual(_task.Title, result1.ConflictingTasks[0]);
        Assert.IsTrue(result1.Message.Contains(limitedResource.Name));
        Assert.IsTrue(result1.Message.Contains(_task.Title));
        Assert.AreEqual(0, conflictingTask.Dependencies.Count);

        ResourceConflictDto result2 =
            _resourceService.CheckAndResolveConflicts(conflictingTask, _project.Id, autoResolve: true);
        Assert.IsTrue(result2.HasConflicts);
        Assert.AreEqual(1, result2.ConflictingTasks.Count);
        Assert.AreEqual(_task.Title, result2.ConflictingTasks[0]);
        Assert.IsTrue(conflictingTask.Dependencies.Contains(_task.Title));

        conflictingTask.Dependencies.Clear();
        conflictingTask.Dependencies.Add(_task.Title);
        _resourceService.CheckAndResolveConflicts(conflictingTask, _project.Id, autoResolve: true);
        Assert.AreEqual(1, conflictingTask.Dependencies.Where(d => d == _task.Title).Count());
    }

    [TestMethod]
    public void CheckAndResolveConflicts_EdgeCases_HandlesCorrectly()
    {
        ResourceType limitedType = new ResourceType { Id = 8, Name = "Limited Type" };
        _resourceTypeRepository.Add(limitedType);

        Resource limitedResource = new Resource
        {
            Name = "Limited Resource",
            Description = "Resource with limited quantity",
            Type = limitedType,
            Quantity = 2
        };
        _resourceRepository.Add(limitedResource);

        TaskResource taskResource = new TaskResource
        {
            Resource = limitedResource,
            Quantity = 1
        };
        _task.Resources = new List<TaskResource> { taskResource };
        _task.EarlyStart = DateTime.Now.AddDays(1);
        _task.EarlyFinish = DateTime.Now.AddDays(5);
        _task.Status = Status.Pending;

        _project.Tasks.Clear();
        _project.Tasks.Add(_task);

        Task secondTask = new Task()
        {
            Title = "Blocking Task",
            Description = "Another conflicting task",
            Duration = 3,
            Status = Status.Pending,
            EarlyStart = DateTime.Now.AddDays(2),
            EarlyFinish = DateTime.Now.AddDays(6),
            LateStart = DateTime.Now.AddDays(2),
            LateFinish = DateTime.Now.AddDays(6),
            Resources = new List<TaskResource>
            {
                new TaskResource { Resource = limitedResource, Quantity = 1 }
            }
        };
        _taskRepository.Add(secondTask);
        _project.Tasks.Add(secondTask);

        TaskResourceDataDTO taskResourceDto = new TaskResourceDataDTO
        {
            TaskTitle = "Edge Case Task",
            ResourceId = limitedResource.Id,
            Quantity = 1
        };

        TaskDataDTO edgeCaseTask = new TaskDataDTO
        {
            Title = "Edge Case Task",
            Description = "Task for testing edge cases",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { taskResourceDto }
        };

        ResourceConflictDto result =
            _resourceService.CheckAndResolveConflicts(edgeCaseTask, _project.Id, autoResolve: true);

        Assert.IsTrue(result.HasConflicts);
        Assert.IsTrue(result.ConflictingTasks.Count >= 1);
        Assert.IsTrue(edgeCaseTask.Dependencies.Count >= 1);
    }

    [TestMethod]
    public void GetResourceStatsByProject_ShouldReturnStatsForProject()
    {
        ResourceType testType = new ResourceType { Id = 15, Name = "Test Type" };
        _resourceTypeRepository.Add(testType);

        Resource testResource = new Resource
        {
            Name = "Test Resource",
            Description = "Test resource description",
            Type = testType,
            Quantity = 5
        };
        _resourceRepository.Add(testResource);

        Task testTask = new Task
        {
            Title = "Test Task For Stats",
            Description = "Test task description",
            Duration = 3,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(3),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(3)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = testTask,
            Resource = testResource,
            Quantity = 2
        };

        testTask.Resources = new List<TaskResource> { taskResource };
        _taskRepository.Add(testTask);

        Project testProject = new Project
        {
            Id = 100,
            Name = "Stats Test Project",
            Description = "Project for testing stats",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { testTask }
        };
        _projectRepository.Add(testProject);

        List<ResourceStatsDto> result = _resourceService.GetResourceStatsByProject(100);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);

        ResourceStatsDto stat = result[0];
        Assert.AreEqual("Test Resource", stat.Name);
        Assert.AreEqual("Test resource description", stat.Description);
        Assert.AreEqual("Test Type", stat.Type);
        Assert.AreEqual(2, stat.Quantity);
        Assert.AreEqual("Test Task For Stats", stat.TaskName);
        Assert.IsTrue(stat.UsagePeriod.Contains(DateTime.Today.ToString("yyyy-MM-dd")));
        Assert.IsTrue(stat.UsagePeriod.Contains(DateTime.Today.AddDays(3).ToString("yyyy-MM-dd")));
    }

    [TestMethod]
    public void GetResourceStatsByProject_WithMultipleTasksAndResources_ShouldReturnAllStats()
    {
        ResourceType type1 = new ResourceType { Id = 16, Name = "Type 1" };
        ResourceType type2 = new ResourceType { Id = 17, Name = "Type 2" };
        _resourceTypeRepository.Add(type1);
        _resourceTypeRepository.Add(type2);

        Resource resource1 = new Resource
        {
            Name = "Resource 1",
            Description = "First resource",
            Type = type1,
            Quantity = 10
        };

        Resource resource2 = new Resource
        {
            Name = "Resource 2",
            Description = "Second resource",
            Type = type2,
            Quantity = 5
        };
        _resourceRepository.Add(resource1);
        _resourceRepository.Add(resource2);

        Task task1 = new Task
        {
            Title = "Task 1",
            Description = "First task",
            Duration = 2,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(2),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(2)
        };

        Task task2 = new Task
        {
            Title = "Task 2",
            Description = "Second task",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Today.AddDays(3),
            EarlyFinish = DateTime.Today.AddDays(4),
            LateStart = DateTime.Today.AddDays(3),
            LateFinish = DateTime.Today.AddDays(4)
        };

        TaskResource taskResource1 = new TaskResource
        {
            Task = task1,
            Resource = resource1,
            Quantity = 3
        };

        TaskResource taskResource2 = new TaskResource
        {
            Task = task2,
            Resource = resource2,
            Quantity = 1
        };

        task1.Resources = new List<TaskResource> { taskResource1 };
        task2.Resources = new List<TaskResource> { taskResource2 };
        _taskRepository.Add(task1);
        _taskRepository.Add(task2);

        Project multiProject = new Project
        {
            Id = 101,
            Name = "Multi Stats Project",
            Description = "Project with multiple tasks and resources",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { task1, task2 }
        };
        _projectRepository.Add(multiProject);

        List<ResourceStatsDto> result = _resourceService.GetResourceStatsByProject(101);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);

        ResourceStatsDto stat1 = result.FirstOrDefault(r => r.Name == "Resource 1");
        ResourceStatsDto stat2 = result.FirstOrDefault(r => r.Name == "Resource 2");

        Assert.IsNotNull(stat1);
        Assert.IsNotNull(stat2);

        Assert.AreEqual("First resource", stat1.Description);
        Assert.AreEqual("Type 1", stat1.Type);
        Assert.AreEqual(3, stat1.Quantity);
        Assert.AreEqual("Task 1", stat1.TaskName);

        Assert.AreEqual("Second resource", stat2.Description);
        Assert.AreEqual("Type 2", stat2.Type);
        Assert.AreEqual(1, stat2.Quantity);
        Assert.AreEqual("Task 2", stat2.TaskName);
    }

    [TestMethod]
    public void GetResourceStatsByProject_EmptyProject_ShouldReturnEmptyList()
    {
        Project emptyProject = new Project
        {
            Id = 102,
            Name = "Empty Project",
            Description = "Project with no tasks",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task>()
        };
        _projectRepository.Add(emptyProject);

        List<ResourceStatsDto> result = _resourceService.GetResourceStatsByProject(102);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetResourceStatsByProject_TasksWithoutResources_ShouldReturnEmptyList()
    {
        Task taskWithoutResources = new Task
        {
            Title = "Task Without Resources",
            Description = "Task that doesn't use any resources",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(1),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(1),
            Resources = new List<TaskResource>()
        };
        _taskRepository.Add(taskWithoutResources);

        Project projectWithoutResourceUsage = new Project
        {
            Id = 103,
            Name = "Project Without Resource Usage",
            Description = "Project with tasks but no resource usage",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { taskWithoutResources }
        };
        _projectRepository.Add(projectWithoutResourceUsage);

        List<ResourceStatsDto> result = _resourceService.GetResourceStatsByProject(103);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void UpdateResource_ReducingQuantity_ShouldValidateUsage()
    {
        ResourceType testType = new ResourceType { Id = 18, Name = "Usage Type" };
        _resourceTypeRepository.Add(testType);

        Resource testResource = new Resource
        {
            Name = "Usage Resource",
            Description = "Resource for usage testing",
            Type = testType,
            Quantity = 10
        };
        _resourceRepository.Add(testResource);

        Task activeTask = new Task
        {
            Title = "Active Task",
            Description = "Active task using resource",
            Duration = 2,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(2),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(2)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = activeTask,
            Resource = testResource,
            Quantity = 3
        };

        activeTask.Resources = new List<TaskResource> { taskResource };
        _taskRepository.Add(activeTask);

        Project testProject = new Project
        {
            Id = 104,
            Name = "Usage Test Project",
            Description = "Project for testing usage",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { activeTask }
        };
        _projectRepository.Add(testProject);

        ResourceDataDto updateDto = new ResourceDataDto
        {
            Id = testResource.Id,
            Name = testResource.Name,
            Description = testResource.Description,
            TypeResource = testType.Id,
            Quantity = 2
        };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(
            () => _resourceService.UpdateResource(updateDto));

        Assert.IsTrue(exception.Message.Contains("Cannot reduce resource quantity to 2"));
        Assert.IsTrue(exception.Message.Contains("Currently 3 units are being used"));
    }

    [TestMethod]
    public void UpdateResource_ReducingQuantityWithMultipleTasks_ShouldValidateTotal()
    {
        ResourceType testType = new ResourceType { Id = 19, Name = "Multi Usage Type" };
        _resourceTypeRepository.Add(testType);

        Resource sharedResource = new Resource
        {
            Name = "Shared Resource",
            Description = "Resource shared across tasks",
            Type = testType,
            Quantity = 20
        };
        _resourceRepository.Add(sharedResource);

        Task task1 = new Task
        {
            Title = "Task 1 Usage",
            Description = "First task using resource",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(1),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(1)
        };

        Task task2 = new Task
        {
            Title = "Task 2 Usage",
            Description = "Second task using resource",
            Duration = 2,
            Status = Status.Blocked,
            EarlyStart = DateTime.Today.AddDays(2),
            EarlyFinish = DateTime.Today.AddDays(4),
            LateStart = DateTime.Today.AddDays(2),
            LateFinish = DateTime.Today.AddDays(4)
        };

        TaskResource taskResource1 = new TaskResource
        {
            Task = task1,
            Resource = sharedResource,
            Quantity = 5
        };

        TaskResource taskResource2 = new TaskResource
        {
            Task = task2,
            Resource = sharedResource,
            Quantity = 7
        };

        task1.Resources = new List<TaskResource> { taskResource1 };
        task2.Resources = new List<TaskResource> { taskResource2 };
        _taskRepository.Add(task1);
        _taskRepository.Add(task2);

        Project multiUsageProject = new Project
        {
            Id = 105,
            Name = "Multi Usage Project",
            Description = "Project with multiple resource usage",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { task1, task2 }
        };
        _projectRepository.Add(multiUsageProject);

        ResourceDataDto updateDto = new ResourceDataDto
        {
            Id = sharedResource.Id,
            Name = sharedResource.Name,
            Description = sharedResource.Description,
            TypeResource = testType.Id,
            Quantity = 10
        };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(
            () => _resourceService.UpdateResource(updateDto));

        Assert.IsTrue(exception.Message.Contains("Cannot reduce resource quantity to 10"));
        Assert.IsTrue(exception.Message.Contains("Currently 12 units are being used"));
    }

    [TestMethod]
    public void UpdateResource_WithCompletedTasks_ShouldIgnoreCompletedUsage()
    {
        ResourceType testType = new ResourceType { Id = 20, Name = "Completed Type" };
        _resourceTypeRepository.Add(testType);

        Resource completedResource = new Resource
        {
            Name = "Completed Resource",
            Description = "Resource with completed tasks",
            Type = testType,
            Quantity = 15
        };
        _resourceRepository.Add(completedResource);

        Task activeTask = new Task
        {
            Title = "Active Task For Completed Test",
            Description = "Active task",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(1),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(1)
        };

        Task completedTask = new Task
        {
            Title = "Completed Task",
            Description = "Completed task",
            Duration = 2,
            Status = Status.Completed,
            EarlyStart = DateTime.Today.AddDays(-3),
            EarlyFinish = DateTime.Today.AddDays(-1),
            LateStart = DateTime.Today.AddDays(-3),
            LateFinish = DateTime.Today.AddDays(-1)
        };

        TaskResource activeTaskResource = new TaskResource
        {
            Task = activeTask,
            Resource = completedResource,
            Quantity = 4
        };

        TaskResource completedTaskResource = new TaskResource
        {
            Task = completedTask,
            Resource = completedResource,
            Quantity = 8
        };

        activeTask.Resources = new List<TaskResource> { activeTaskResource };
        completedTask.Resources = new List<TaskResource> { completedTaskResource };
        _taskRepository.Add(activeTask);
        _taskRepository.Add(completedTask);

        Project completedTestProject = new Project
        {
            Id = 106,
            Name = "Completed Test Project",
            Description = "Project with completed and active tasks",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { activeTask, completedTask }
        };
        _projectRepository.Add(completedTestProject);

        ResourceDataDto updateDto = new ResourceDataDto
        {
            Id = completedResource.Id,
            Name = completedResource.Name,
            Description = completedResource.Description,
            TypeResource = testType.Id,
            Quantity = 3
        };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(
            () => _resourceService.UpdateResource(updateDto));

        Assert.IsTrue(exception.Message.Contains("Cannot reduce resource quantity to 3"));
        Assert.IsTrue(exception.Message.Contains("Currently 4 units are being used"));
    }

    [TestMethod]
    public void UpdateResource_WithUnusedResource_ShouldAllowQuantityReduction()
    {
        ResourceType unusedType = new ResourceType { Id = 21, Name = "Unused Type" };
        _resourceTypeRepository.Add(unusedType);

        Resource unusedResource = new Resource
        {
            Name = "Unused Resource",
            Description = "Resource not used by any task",
            Type = unusedType,
            Quantity = 10
        };
        _resourceRepository.Add(unusedResource);

        ResourceDataDto updateDto = new ResourceDataDto
        {
            Id = unusedResource.Id,
            Name = unusedResource.Name,
            Description = unusedResource.Description,
            TypeResource = unusedType.Id,
            Quantity = 1
        };

        Resource result = _resourceService.UpdateResource(updateDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Quantity);
    }

    [TestMethod]
    public void UpdateResource_IncreasingQuantity_ShouldAllowUpdate()
    {
        ResourceType testType = new ResourceType { Id = 22, Name = "Increase Type" };
        _resourceTypeRepository.Add(testType);

        Resource increaseResource = new Resource
        {
            Name = "Increase Resource",
            Description = "Resource for increasing quantity",
            Type = testType,
            Quantity = 5
        };
        _resourceRepository.Add(increaseResource);

        Task task = new Task
        {
            Title = "Task Using Resource",
            Description = "Task using the resource",
            Duration = 1,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(1),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(1)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = task,
            Resource = increaseResource,
            Quantity = 3
        };

        task.Resources = new List<TaskResource> { taskResource };
        _taskRepository.Add(task);

        Project project = new Project
        {
            Id = 107,
            Name = "Increase Test Project",
            Description = "Project for testing quantity increase",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { task }
        };
        _projectRepository.Add(project);

        ResourceDataDto updateDto = new ResourceDataDto
        {
            Id = increaseResource.Id,
            Name = increaseResource.Name,
            Description = increaseResource.Description,
            TypeResource = testType.Id,
            Quantity = 10
        };

        Resource result = _resourceService.UpdateResource(updateDto);

        Assert.IsNotNull(result);
        Assert.AreEqual(10, result.Quantity);
    }

    [TestMethod]
    public void RemoveResource_WithCompletedTasks_ShouldRemoveReferences()
    {
        ResourceType testType = new ResourceType { Id = 23, Name = "Removal Type" };
        _resourceTypeRepository.Add(testType);

        Resource resourceToRemove = new Resource
        {
            Name = "Resource To Remove",
            Description = "Resource that will be removed",
            Type = testType,
            Quantity = 5
        };
        _resourceRepository.Add(resourceToRemove);

        Task completedTask = new Task
        {
            Title = "Completed Task Using Resource",
            Description = "Completed task description",
            Duration = 2,
            Status = Status.Completed,
            EarlyStart = DateTime.Today.AddDays(-5),
            EarlyFinish = DateTime.Today.AddDays(-3),
            LateStart = DateTime.Today.AddDays(-5),
            LateFinish = DateTime.Today.AddDays(-3)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = completedTask,
            Resource = resourceToRemove,
            Quantity = 2
        };

        completedTask.Resources = new List<TaskResource> { taskResource };
        _taskRepository.Add(completedTask);

        Project testProject = new Project
        {
            Id = 108,
            Name = "Removal Test Project",
            Description = "Project for testing resource removal",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { completedTask }
        };
        _projectRepository.Add(testProject);

        GetResourceDto resourceDto = new GetResourceDto { Name = "Resource To Remove" };

        _resourceService.RemoveResource(resourceDto);

        Assert.AreEqual(0, _resourceRepository.FindAll().Count(r => r.Name == "Resource To Remove"));

        Project updatedProject = _projectRepository.Find(p => p.Id == 108);
        Task updatedTask = updatedProject.Tasks.First();
        Assert.AreEqual(0, updatedTask.Resources.Count);
    }

    [TestMethod]
    public void RemoveResource_WithActiveTasksUsage_ShouldThrowException()
    {
        ResourceType testType = new ResourceType { Id = 24, Name = "Active Usage Type" };
        _resourceTypeRepository.Add(testType);

        Resource resourceInUse = new Resource
        {
            Name = "Resource In Use",
            Description = "Resource currently being used",
            Type = testType,
            Quantity = 3
        };
        _resourceRepository.Add(resourceInUse);

        Task activeTask = new Task
        {
            Title = "Active Task Using Resource",
            Description = "Active task description",
            Duration = 3,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(3),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(3)
        };

        TaskResource taskResource = new TaskResource
        {
            Task = activeTask,
            Resource = resourceInUse,
            Quantity = 1
        };

        activeTask.Resources = new List<TaskResource> { taskResource };
        _taskRepository.Add(activeTask);

        Project activeProject = new Project
        {
            Id = 109,
            Name = "Active Usage Project",
            Description = "Project with active resource usage",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { activeTask }
        };
        _projectRepository.Add(activeProject);

        GetResourceDto resourceDto = new GetResourceDto { Name = "Resource In Use" };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(
            () => _resourceService.RemoveResource(resourceDto));

        Assert.IsTrue(exception.Message.Contains("Cannot delete resource 'Resource In Use'"));
        Assert.IsTrue(exception.Message.Contains("Active Task Using Resource"));
    }

    [TestMethod]
    public void RemoveResource_WithMixedCompletedAndActiveTasks_ShouldThrowException()
    {
        ResourceType testType = new ResourceType { Id = 25, Name = "Mixed Usage Type" };
        _resourceTypeRepository.Add(testType);

        Resource mixedResource = new Resource
        {
            Name = "Mixed Usage Resource",
            Description = "Resource with mixed usage",
            Type = testType,
            Quantity = 8
        };
        _resourceRepository.Add(mixedResource);

        Task completedTask = new Task
        {
            Title = "Completed Task Mixed",
            Description = "Completed task",
            Duration = 1,
            Status = Status.Completed,
            EarlyStart = DateTime.Today.AddDays(-2),
            EarlyFinish = DateTime.Today.AddDays(-1),
            LateStart = DateTime.Today.AddDays(-2),
            LateFinish = DateTime.Today.AddDays(-1)
        };

        Task activeTask = new Task
        {
            Title = "Active Task Mixed",
            Description = "Active task",
            Duration = 2,
            Status = Status.Blocked,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(2),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(2)
        };

        TaskResource completedTaskResource = new TaskResource
        {
            Task = completedTask,
            Resource = mixedResource,
            Quantity = 3
        };

        TaskResource activeTaskResource = new TaskResource
        {
            Task = activeTask,
            Resource = mixedResource,
            Quantity = 2
        };

        completedTask.Resources = new List<TaskResource> { completedTaskResource };
        activeTask.Resources = new List<TaskResource> { activeTaskResource };
        _taskRepository.Add(completedTask);
        _taskRepository.Add(activeTask);

        Project mixedProject = new Project
        {
            Id = 110,
            Name = "Mixed Usage Project",
            Description = "Project with mixed resource usage",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { completedTask, activeTask }
        };
        _projectRepository.Add(mixedProject);

        GetResourceDto resourceDto = new GetResourceDto { Name = "Mixed Usage Resource" };

        InvalidOperationException exception = Assert.ThrowsException<InvalidOperationException>(
            () => _resourceService.RemoveResource(resourceDto));

        Assert.IsTrue(exception.Message.Contains("Cannot delete resource 'Mixed Usage Resource'"));
        Assert.IsTrue(exception.Message.Contains("Active Task Mixed"));
        Assert.IsFalse(exception.Message.Contains("Completed Task Mixed"));
    }

    [TestMethod]
    public void RemoveResource_WithMultipleProjects_ShouldRemoveAllReferences()
    {
        ResourceType testType = new ResourceType { Id = 26, Name = "Multi Project Type" };
        _resourceTypeRepository.Add(testType);

        Resource multiProjectResource = new Resource
        {
            Name = "Multi Project Resource",
            Description = "Resource used across multiple projects",
            Type = testType,
            Quantity = 6
        };
        _resourceRepository.Add(multiProjectResource);

        Task task1 = new Task
        {
            Title = "Task In Project 1",
            Description = "Task from first project",
            Duration = 1,
            Status = Status.Completed,
            EarlyStart = DateTime.Today.AddDays(-3),
            EarlyFinish = DateTime.Today.AddDays(-2),
            LateStart = DateTime.Today.AddDays(-3),
            LateFinish = DateTime.Today.AddDays(-2)
        };

        Task task2 = new Task
        {
            Title = "Task In Project 2",
            Description = "Task from second project",
            Duration = 2,
            Status = Status.Completed,
            EarlyStart = DateTime.Today.AddDays(-5),
            EarlyFinish = DateTime.Today.AddDays(-3),
            LateStart = DateTime.Today.AddDays(-5),
            LateFinish = DateTime.Today.AddDays(-3)
        };

        TaskResource taskResource1 = new TaskResource
        {
            Task = task1,
            Resource = multiProjectResource,
            Quantity = 2
        };

        TaskResource taskResource2 = new TaskResource
        {
            Task = task2,
            Resource = multiProjectResource,
            Quantity = 1
        };

        task1.Resources = new List<TaskResource> { taskResource1 };
        task2.Resources = new List<TaskResource> { taskResource2 };
        _taskRepository.Add(task1);
        _taskRepository.Add(task2);

        Project project1 = new Project
        {
            Id = 111,
            Name = "Multi Project 1",
            Description = "First project using resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { task1 }
        };

        Project project2 = new Project
        {
            Id = 112,
            Name = "Multi Project 2",
            Description = "Second project using resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { task2 }
        };

        _projectRepository.Add(project1);
        _projectRepository.Add(project2);

        GetResourceDto resourceDto = new GetResourceDto { Name = "Multi Project Resource" };

        _resourceService.RemoveResource(resourceDto);

        Assert.AreEqual(0, _resourceRepository.FindAll().Count(r => r.Name == "Multi Project Resource"));

        Project updatedProject1 = _projectRepository.Find(p => p.Id == 111);
        Project updatedProject2 = _projectRepository.Find(p => p.Id == 112);

        Assert.AreEqual(0, updatedProject1.Tasks.First().Resources.Count);
        Assert.AreEqual(0, updatedProject2.Tasks.First().Resources.Count);
    }

    [TestMethod]
    public void RemoveResource_NonExistentResource_ShouldThrowException()
    {
        GetResourceDto nonExistentResource = new GetResourceDto { Name = "Non Existent Resource" };

        ArgumentException exception = Assert.ThrowsException<ArgumentException>(
            () => _resourceService.RemoveResource(nonExistentResource));

        Assert.AreEqual("Resource 'Non Existent Resource' not found", exception.Message);
    }

    [TestMethod]
    public void CheckAndResolveConflicts_WithExclusiveResource_ShouldDetectCorrectResource()
    {
        ResourceType exclusiveType = new ResourceType { Id = 27, Name = "Exclusive Info Type" };
        _resourceTypeRepository.Add(exclusiveType);

        Resource exclusiveResource = new Resource
        {
            Name = "Exclusive Info Resource",
            Description = "Exclusive resource for testing info",
            Type = exclusiveType,
            Quantity = 2
        };

        Project exclusiveProject = new Project
        {
            Id = 113,
            Name = "Exclusive Info Project",
            Description = "Project with exclusive resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            ExclusiveResources = new List<Resource> { exclusiveResource },
            Tasks = new List<Task>()
        };
        _projectRepository.Add(exclusiveProject);

        Task existingTask = new Task
        {
            Title = "Existing Exclusive Task",
            Description = "Task using exclusive resource",
            Duration = 2,
            Status = Status.Pending,
            EarlyStart = DateTime.Today,
            EarlyFinish = DateTime.Today.AddDays(2),
            LateStart = DateTime.Today,
            LateFinish = DateTime.Today.AddDays(2)
        };

        TaskResource existingTaskResource = new TaskResource
        {
            Task = existingTask,
            Resource = exclusiveResource,
            Quantity = 2
        };

        existingTask.Resources = new List<TaskResource> { existingTaskResource };
        _taskRepository.Add(existingTask);
        exclusiveProject.Tasks.Add(existingTask);

        TaskResourceDataDTO conflictingResource = new TaskResourceDataDTO
        {
            TaskTitle = "New Conflicting Task",
            ResourceId = exclusiveResource.Id,
            Quantity = 1
        };

        TaskDataDTO newTask = new TaskDataDTO
        {
            Title = "New Conflicting Task",
            Description = "New task that conflicts with exclusive resource",
            Duration = 1,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { conflictingResource }
        };

        ResourceConflictDto result = _resourceService.CheckAndResolveConflicts(newTask, 113, false);

        Assert.IsTrue(result.HasConflicts);
        Assert.IsTrue(result.Message.Contains("Exclusive Info Resource"));
        Assert.IsTrue(result.Message.Contains("Existing Exclusive Task"));
    }

    [TestMethod]
    public void CheckAndResolveConflicts_WithSystemResource_ShouldDetectCorrectResource()
    {
        ResourceType systemType = new ResourceType { Id = 28, Name = "System Info Type" };
        _resourceTypeRepository.Add(systemType);

        Resource systemResource = new Resource
        {
            Name = "System Info Resource",
            Description = "System resource for testing info",
            Type = systemType,
            Quantity = 3
        };
        _resourceRepository.Add(systemResource);

        Task existingTask = new Task
        {
            Title = "Existing System Task",
            Description = "Task using system resource",
            Duration = 3,
            Status = Status.Pending,
            EarlyStart = DateTime.Today.AddDays(1),
            EarlyFinish = DateTime.Today.AddDays(4),
            LateStart = DateTime.Today.AddDays(1),
            LateFinish = DateTime.Today.AddDays(4)
        };

        TaskResource existingTaskResource = new TaskResource
        {
            Task = existingTask,
            Resource = systemResource,
            Quantity = 3
        };

        existingTask.Resources = new List<TaskResource> { existingTaskResource };
        _taskRepository.Add(existingTask);

        Project systemProject = new Project
        {
            Id = 114,
            Name = "System Info Project",
            Description = "Project using system resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task> { existingTask }
        };
        _projectRepository.Add(systemProject);

        TaskResourceDataDTO conflictingResource = new TaskResourceDataDTO
        {
            TaskTitle = "New System Task",
            ResourceId = systemResource.Id,
            Quantity = 1
        };

        TaskDataDTO newTask = new TaskDataDTO
        {
            Title = "New System Task",
            Description = "New task that conflicts with system resource",
            Duration = 2,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { conflictingResource }
        };

        ResourceConflictDto result = _resourceService.CheckAndResolveConflicts(newTask, 114, false);

        Assert.IsTrue(result.HasConflicts);
        Assert.IsTrue(result.Message.Contains("System Info Resource"));
        Assert.IsTrue(result.Message.Contains("Existing System Task"));
    }

    [TestMethod]
    public void CheckAndResolveConflicts_WithNonExistentExclusiveResource_ShouldReturnUnknownResource()
    {
        TaskResourceDataDTO nonExistentResource = new TaskResourceDataDTO
        {
            TaskTitle = "Task With Non Existent Resource",
            ResourceId = 99999,
            Quantity = 1
        };

        TaskDataDTO taskWithNonExistentResource = new TaskDataDTO
        {
            Title = "Task With Non Existent Resource",
            Description = "Task trying to use non-existent exclusive resource",
            Duration = 1,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { nonExistentResource }
        };

        ResourceConflictDto result =
            _resourceService.CheckAndResolveConflicts(taskWithNonExistentResource, _project.Id, false);

        Assert.IsFalse(result.HasConflicts);
    }

    [TestMethod]
    public void CheckAndResolveConflicts_WithNonExistentSystemResource_ShouldReturnUnknownResource()
    {
        TaskResourceDataDTO nonExistentResource = new TaskResourceDataDTO
        {
            TaskTitle = "Task With Non Existent System Resource",
            ResourceId = 88888,
            Quantity = 1
        };

        TaskDataDTO taskWithNonExistentResource = new TaskDataDTO
        {
            Title = "Task With Non Existent System Resource",
            Description = "Task trying to use non-existent system resource",
            Duration = 1,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { nonExistentResource }
        };

        Project newProject = new Project
        {
            Id = 115,
            Name = "Non Existent Resource Project",
            Description = "Project for testing non-existent resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task>()
        };
        _projectRepository.Add(newProject);

        ResourceConflictDto result = _resourceService.CheckAndResolveConflicts(taskWithNonExistentResource, 115, false);

        Assert.IsFalse(result.HasConflicts);
    }

    [TestMethod]
    public void CheckAndResolveConflicts_WithAvailableExclusiveResource_ShouldNotDetectConflicts()
    {
        ResourceType availableType = new ResourceType { Id = 29, Name = "Available Exclusive Type" };
        _resourceTypeRepository.Add(availableType);

        Resource availableResource = new Resource
        {
            Name = "Available Exclusive Resource",
            Description = "Available exclusive resource",
            Type = availableType,
            Quantity = 5
        };

        Project availableProject = new Project
        {
            Id = 116,
            Name = "Available Exclusive Project",
            Description = "Project with available exclusive resource",
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            ProjectRoles = new List<ProjectRole>(),
            ExclusiveResources = new List<Resource> { availableResource },
            Tasks = new List<Task>()
        };
        _projectRepository.Add(availableProject);

        TaskResourceDataDTO availableResourceDto = new TaskResourceDataDTO
        {
            TaskTitle = "Task Using Available Resource",
            ResourceId = availableResource.Id,
            Quantity = 2
        };

        TaskDataDTO taskWithAvailableResource = new TaskDataDTO
        {
            Title = "Task Using Available Resource",
            Description = "Task using available exclusive resource",
            Duration = 2,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO> { availableResourceDto }
        };

        ResourceConflictDto result = _resourceService.CheckAndResolveConflicts(taskWithAvailableResource, 116, false);

        Assert.IsFalse(result.HasConflicts);
        Assert.AreEqual(0, result.ConflictingTasks.Count);
    }
}