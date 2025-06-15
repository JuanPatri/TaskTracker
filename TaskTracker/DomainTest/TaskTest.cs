using BusinessLogicTest.Context;
using DTOs.TaskDTOs;
using Domain;
using Enums;
using DTOs.TaskResourceDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Service;
using Task = Domain.Task;

namespace DomainTest;


[TestClass]
public class TaskTest
{
    private Task _task;
    private TaskService _taskService;
    private TaskRepository _taskRepository;
    private ResourceTypeRepository _resourceTypeRespository;
    private ProjectRepository _projectRepository;
    private ProjectService _projectService;
    private CriticalPathService _criticalPathService;
    private UserRepository _userRepository;
    private UserService _userService;
    private ResourceRepository _resourceRespository;
    private SqlContext _sqlContext;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _taskRepository = new TaskRepository(_sqlContext);
        _resourceRespository = new ResourceRepository(_sqlContext);
        _projectRepository = new ProjectRepository(_sqlContext);
        _userRepository = new UserRepository(_sqlContext);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);
        _userService = new UserService(_userRepository);
        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRespository, _userRepository, _userService, _criticalPathService);
        _task = new Task();
        _taskService = new TaskService(_taskRepository, _resourceRespository, _projectRepository, _projectService, _criticalPathService);

        _task = new Task
        {
            Title = "Test Task",
            Description = "This is a test task",
            Duration = 5,
            Status = Status.Pending,
            EarlyStart = DateTime.Now,
            EarlyFinish = DateTime.Now.AddDays(5),
            LateStart = DateTime.Now,
            LateFinish = DateTime.Now.AddDays(5),
            DateCompleated = null,
            Resources = new List<TaskResource>(),
            Dependencies = new List<TaskDependency>()
        };
    }

    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_task);
    }

    [TestMethod]
    public void SetValidTitleUpdatesValueCorrectly()
    {
        string expectedTitle = "Valid Title";
        _task.Title = expectedTitle;
        Assert.AreEqual(expectedTitle, _task.Title);
    }

    [TestMethod]
    public void PutTitleNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = null);
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void PutTitleWhitespaceReturnAnExceptionTest()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = " ");
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateDescrptionForTaskTest()
    {
        _task.Description = "Description";
        Assert.AreEqual("Description", _task.Description);
    }

    [TestMethod]
    public void SetDurationForTaskTest()
    {
        int durationTask = 1;

        _task.Duration = durationTask;

        Assert.AreEqual(durationTask, _task.Duration);
    }

    [TestMethod]
    public void SetStatusForTaskTest()
    {
        _task.Status = Status.Pending;
        Assert.AreEqual(Status.Pending, _task.Status);
    }

    [TestMethod]
    public void SetDescriptionNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Description = null);
        Assert.AreEqual("The description cannot be empty", ex.Message);
    }

    [TestMethod]
    public void SetDurationNullReturnsAnException()
    {
        int durationTask = 0;

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Duration = durationTask);
        Assert.AreEqual("The duration must be at least 1 day", ex.Message);
    }

    [TestMethod]
    public void SetTaskResourcesTaskTest()
    {
        Resource res = new Resource();
        TaskResource taskResource = new TaskResource()
        {
            Task = _task,
            Resource = res,
            Quantity = 2,
        };
    
        List<TaskResource> taskResources = new List<TaskResource>
        {
            taskResource
        };

        _task.Resources = taskResources;
        Assert.AreEqual(taskResources, _task.Resources);
    }

    [TestMethod]
    public void SetDependencies()
    {
        List<TaskDependency> dependencies = new List<TaskDependency>
        {
            new TaskDependency 
            {
                Task = _task,
                Dependency = new Task { Title = "Task 1", Description = "Description 1" }
            },
            new TaskDependency 
            {
                Task = _task,
                Dependency = new Task { Title = "Task 2", Description = "Description 2" }
            }
        };
    
        _task.Dependencies = dependencies;
    
        Assert.AreEqual(dependencies, _task.Dependencies);
    }

    [TestMethod]
    public void FromDtoShouldCreateTaskWithCorrectValues()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task 1",
            Description = "Description of Task 1",
            Duration = 1,
            Status = Status.Blocked,
            Dependencies = new List<string> { "Task 1", "Task 2" },
            Resources = new List<TaskResourceDataDTO> 
            { 
                new TaskResourceDataDTO
                {
                    TaskTitle = "Task 1",
                    ResourceId = 1,
                    Quantity = 1
                }
            }
        };

        List<Task> dependencies = new List<Task>
        {
            new Task { Title = "Task 3", Description = "Description of Task 3" }
        };

        List<TaskResource> taskResources = new List<TaskResource>
        {
            new TaskResource()
            {
                Task = new Task { Title = "Task 1" },
                Resource = new Resource { Name = "Resource 1" },
                Quantity = 1,
            }
        };

        Task task = _taskService.FromDto(taskDto, taskResources, dependencies);

        Assert.AreEqual("Task 1", task.Title);
        Assert.AreEqual("Description of Task 1", task.Description);
        Assert.AreEqual(1, task.Duration);
        Assert.AreEqual(Status.Blocked, task.Status);
        
        Assert.IsNotNull(task.Resources);
        Assert.AreEqual(1, task.Resources.Count);
        Assert.AreEqual(1, task.Resources[0].Quantity);
        Assert.AreEqual("Resource 1", task.Resources[0].Resource.Name);

        Assert.IsNotNull(task.Dependencies);
        Assert.AreEqual("Task 3", task.Dependencies[0].Title);
    }
    
    [TestMethod]
    public void SetTaskEarlyStart()
    {
        DateTime fecha = new DateTime(2025, 10, 14);
        _task.EarlyStart = fecha;
        Assert.AreEqual(fecha, _task.EarlyStart);
    }
    
    [TestMethod]
    public void SetTaskEarlyFinish()
    {
        DateTime fecha = new DateTime(2025, 10, 15);
        _task.EarlyFinish = fecha;
        Assert.AreEqual(fecha, _task.EarlyFinish);
    }
    
    [TestMethod]
    public void SetTaskDateCompleated()
    {
        DateTime fecha = new DateTime(2025, 10, 16);
        _task.DateCompleated = fecha;
        Assert.AreEqual(fecha, _task.DateCompleated);
    }
    
    [TestMethod]
    public void SetTaskLateStart()
    {
        DateTime fecha = new DateTime(2025, 10, 17);
        _task.LateStart = fecha;
        Assert.AreEqual(fecha, _task.LateStart);
    }
    [TestMethod]
    public void SetTaskLateFinish()
    {
        DateTime fecha = new DateTime(2025, 10, 18);
        _task.LateFinish = fecha;
        Assert.AreEqual(fecha, _task.LateFinish);
    }
    
}