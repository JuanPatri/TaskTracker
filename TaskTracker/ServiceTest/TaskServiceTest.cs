using Domain;
using DTOs.TaskDTOs;
using DTOs.TaskResourceDTOs;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using Service;
using Task = Domain.Task;

namespace ServiceTest;

[TestClass]
public class TaskServiceTest
{
    private  IRepository<Task> _taskRepository;
    private  IRepository<Resource> _resourceRepository;
    private  IRepository<Project> _projectRepository;
    private TaskService _taskService;
    private Project _project;
    private Task _task;
    private Resource _resource;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _resourceRepository = new ResourceRepository();
        _projectRepository = new ProjectRepository();
        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository);

        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            Administrator = new User()
        };
        _projectRepository.Add(_project);

        _task = new Task() { Title = "Test Task", };
        _taskRepository.Add(_task);

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
        //_resourceTypeRepository.Add(_resource.Type);
    }
    
    [TestMethod]
    public void GetTaskDatesFromDto_ShouldCalculateCorrectEarlyStartAndFinishDates()
    {
        _task.EarlyStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        _task.EarlyFinish = _task.EarlyStart.AddDays(5);

        var taskWithoutDeps = new TaskDataDTO
        {
            Title = "No Dependencies Task",
            Description = "Task without any dependencies",
            Duration = 3,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO>()
        };

        var (startNoDeps, finishNoDeps) = _taskService.GetTaskDatesFromDto(taskWithoutDeps, _project.Id);
        
        var expectedProjectStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        Assert.AreEqual(expectedProjectStart, startNoDeps, "Task without dependencies should start at project start date");
        Assert.AreEqual(expectedProjectStart.AddDays(3), finishNoDeps, "Task finish should be start + duration");

        var taskWithDeps = new TaskDataDTO
        {
            Title = "With Dependencies Task",
            Description = "Task with dependencies on existing task",
            Duration = 2,
            Dependencies = new List<string> { _task.Title }, 
            Resources = new List<TaskResourceDataDTO>()
        };

        var (startWithDeps, finishWithDeps) = _taskService.GetTaskDatesFromDto(taskWithDeps, _project.Id);
        
        Assert.AreEqual(_task.EarlyFinish, startWithDeps, "Task with dependencies should start when dependency finishes");
        Assert.AreEqual(_task.EarlyFinish.AddDays(2), finishWithDeps, "Task finish should be dependency finish + duration");

        var taskWithBadDeps = new TaskDataDTO
        {
            Title = "Bad Dependencies Task",
            Description = "Task with non-existent dependencies",
            Duration = 1,
            Dependencies = new List<string> { "NonExistentTask" },
            Resources = new List<TaskResourceDataDTO>()
        };

        var (startBadDeps, finishBadDeps) = _taskService.GetTaskDatesFromDto(taskWithBadDeps, _project.Id);
        
        Assert.AreEqual(expectedProjectStart, startBadDeps, "Task with invalid dependencies should start at project start");
        Assert.AreEqual(expectedProjectStart.AddDays(1), finishBadDeps, "Task finish should be project start + duration");

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _taskService.GetTaskDatesFromDto(taskWithoutDeps, 999));
        
        Assert.AreEqual("Project with ID 999 not found", exception.Message, "Should throw exception with correct message");
    }
    
    [TestMethod]
    public void AddTask_WithExistingDependenciesAndResourcesShouldAddTaskWithCorrectDependenciesAndResources()
    {
        Task dependency1 = new Task { Title = "Dependency1" };
        Task dependency2 = new Task { Title = "Dependency2" };
        _taskRepository.Add(dependency1);
        _taskRepository.Add(dependency2);

        Resource existingResource = _resource; 

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Real Dependencies And Resources",
            Description = "Task that has real dependencies and resources",
            Duration = 3,
            Status = Status.Pending,
            Dependencies = new List<string> { "Dependency1", "Dependency2" },
            Resources = new List<TaskResourceDataDTO>
            {
                new TaskResourceDataDTO { TaskTitle = "Task With Real Dependencies And Resources", ResourceId = existingResource.Id, Quantity = 2 }
            }
        };

        Task addedTask = _taskService.AddTask(taskDto);

        Assert.IsNotNull(addedTask);
        Assert.IsNotNull(addedTask.Resources);
        Assert.AreEqual(1, addedTask.Resources.Count); 
        Assert.IsTrue(addedTask.Resources.Any(tr => tr.Resource.Name == "Resource" && tr.Quantity == 2));
    }

    [TestMethod]
    public void AddTask_WithDuplicateTitle_ShouldThrowException()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Test Task",
            Description = "This is a duplicate task",
            Duration = 2,
            Status = Status.Pending,
            Dependencies = new List<string>(),
            Resources = new List<TaskResourceDataDTO>()
        };

        Assert.ThrowsException<Exception>(() => _taskService.AddTask(taskDto));
    }

    [TestMethod]
    public void AddTask_WithNonExistentDependenciesAndResources_ShouldAddTaskWithEmptyCollections()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Nonexistent Dependencies",
            Description = "This task refers to non-existent dependencies and resources",
            Duration = 2,
            Status = Status.Pending,
            Dependencies = new List<string> { "NonexistentDep1", "NonexistentDep2" },
            Resources = new List<TaskResourceDataDTO>
            {
                new TaskResourceDataDTO {TaskTitle = "NonexistentDep1", ResourceId = 99999, Quantity = 1 },
                new TaskResourceDataDTO { TaskTitle = "NonexistentDep2", ResourceId = 99998, Quantity = 2 }
            }
        };

        Task addedTask = _taskService.AddTask(taskDto);
    
        Assert.IsNotNull(addedTask);
        Assert.AreEqual(0, addedTask.Dependencies.Count);
        Assert.AreEqual(0, addedTask.Resources.Count);
    }
    
    [TestMethod]
    public void FindTaskByTitleReturnTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        Assert.AreEqual(_taskService.GetTaskByTitle(taskDto.Title), _task);
    }

    [TestMethod]
    public void FindAllTasksReturnsAllTasks()
    {
        List<Task> tasks = _taskService.GetAllTasks();
        Assert.AreEqual(1, tasks.Count);

        TaskDataDTO task2 = new TaskDataDTO();
        task2.Title = "Test Task 2";
        task2.Description = "This is a test task.";
        task2.Duration = 1;
        task2.Status = Status.Pending;
        _taskService.AddTask(task2);
        tasks = _taskService.GetAllTasks();

        Assert.AreEqual(2, tasks.Count);
    }

    [TestMethod]
    public void UpdateTaskShouldUpdateTask()
    {
        Project project = new Project() { Id = 45, Name = "Updated Test Project" };
        _projectRepository.Add(project);

        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        taskDto.Description = "New Description";
        taskDto.Duration = 1;
        taskDto.Status = Status.Blocked;
        taskDto.Dependencies = new List<string>() { "Task1", "Task2" };

        _taskRepository.Add(new Task()
        {
            Title = "Task1"
        });

        _taskRepository.Add(new Task()
        {
            Title = "Task2"
        });

        _task.Description = "Description";
        Assert.AreEqual(_task.Description, "Description");

        _taskService.UpdateTask(taskDto);

        Assert.AreEqual("New Description", _task.Description);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsFalse_WhenTaskDoesNotExist()
    {
        TaskDataDTO nonExistentTaskDto = new TaskDataDTO
        {
            Title = "Non-Existent Task",
            Description = "This task does not exist in the repository"
        };

        bool result = _taskService.CanMarkTaskAsCompleted(nonExistentTaskDto);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsTrue_WhenAllDependenciesAreCompleted()
    {
        Task dependency1 = new Task
        {
            Title = "Dependency 1",
            Status = Status.Completed
        };
        Task dependency2 = new Task
        {
            Title = "Dependency 2",
            Status = Status.Completed
        };
        _taskRepository.Add(dependency1);
        _taskRepository.Add(dependency2);

        Task taskWithCompletedDependencies = new Task
        {
            Title = "Task With Completed Dependencies",
            Dependencies = new List<Task> { dependency1, dependency2 }
        };
        _taskRepository.Add(taskWithCompletedDependencies);

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Completed Dependencies"
        };

        bool result = _taskService.CanMarkTaskAsCompleted(taskDto);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanMarkTaskAsCompleted_ReturnsFalse_WhenAnyDependencyIsNotCompleted()
    {
        Task completedDependency = new Task
        {
            Title = "Completed Dependency",
            Status = Status.Completed
        };
        Task pendingDependency = new Task
        {
            Title = "Pending Dependency",
            Status = Status.Pending
        };
        _taskRepository.Add(completedDependency);
        _taskRepository.Add(pendingDependency);

        Task taskWithMixedDependencies = new Task
        {
            Title = "Task With Mixed Dependencies",
            Dependencies = new List<Task> { completedDependency, pendingDependency }
        };
        _taskRepository.Add(taskWithMixedDependencies);

        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Task With Mixed Dependencies"
        };

        bool result = _taskService.CanMarkTaskAsCompleted(taskDto);


        Assert.IsFalse(result);
    }

    [TestMethod]
    public void RemoveTaskShouldDeleteTask()
    {
        Assert.AreEqual(1, _taskRepository.FindAll().Count());

        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test Task";
        _taskService.RemoveTask(taskDto);

        Assert.AreEqual(0, _taskRepository.FindAll().Count());
    }

    [TestMethod]
    public void GetTaskDependenciesWithTitleShouldReturnTask()
    {
        Task dependencyTask1 = new Task { Title = "Task1" };
        Task dependencyTask2 = new Task { Title = "Task2" };
        _taskRepository.Add(dependencyTask1);
        _taskRepository.Add(dependencyTask2);

        Task taskWithDependency = new Task
        {
            Title = "MainTask",
            Dependencies = new List<Task> { dependencyTask1 }
        };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask",
            Dependencies = new List<Task> { dependencyTask2 }
        };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<Task> tasks = _taskService.GetTaskDependenciesWithTitleTask(searchList);

        Assert.AreEqual(1, tasks.Count);
        Assert.AreEqual("Task1", tasks[0].Title);
    }

    [TestMethod]
    public void GetTaskDependenciesWithTitleTask_ShouldReturnMatchingTasks()
    {
        Task dependencyTask1 = new Task { Title = "Task1" };
        Task dependencyTask2 = new Task { Title = "Task2" };
        _taskRepository.Add(dependencyTask1);
        _taskRepository.Add(dependencyTask2);

        Task taskWithDependency = new Task
        {
            Title = "MainTask",
            Dependencies = new List<Task> { dependencyTask1 }
        };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask",
            Dependencies = new List<Task> { dependencyTask2 }
        };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<Task> tasks = _taskService.GetTaskDependenciesWithTitleTask(searchList);

        Assert.AreEqual(1, tasks.Count);
        Assert.AreEqual("Task1", tasks[0].Title);
    }

    [TestMethod]
    public void ValidateTaskStatusTest()
    {
        TaskDataDTO taskDto = new TaskDataDTO
        {
            Title = "Test Task",
            Description = "This is a test task.",
            Dependencies = new List<string>() { "Task1", "Task2" }
        };

        Status status = Status.Completed;

        bool isValid = _taskService.ValidateTaskStatus("Test Task", status);
        Assert.IsFalse(isValid);
    }
}