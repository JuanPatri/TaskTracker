using BusinessLogicTest.Context;
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
    private IRepository<Task> _taskRepository;
    private IRepository<Resource> _resourceRepository;
    private IRepository<Project> _projectRepository;
    private TaskService _taskService;
    private Project _project;
    private Task _task;
    private Resource _resource;
    private ProjectService _projectService;
    private UserRepository _userRepository;
    private UserService _userService;
    private CriticalPathService _criticalPathService;
    private ResourceTypeRepository _resourceTypeRepository;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();

        _sqlContext.Database.EnsureDeleted();
        _sqlContext.Database.EnsureCreated();

        _taskRepository = new TaskRepository(_sqlContext);
        _resourceRepository = new ResourceRepository(_sqlContext);
        _projectRepository = new ProjectRepository(_sqlContext);
        _userRepository = new UserRepository(_sqlContext);
        _resourceTypeRepository = new ResourceTypeRepository(_sqlContext);
        _userService = new UserService(_userRepository);
        _criticalPathService = new CriticalPathService(_projectRepository, _taskRepository);

        _projectService = new ProjectService(_taskRepository, _projectRepository, _resourceTypeRepository,
            _userRepository, _userService, _criticalPathService);

        _taskService = new TaskService(_taskRepository, _resourceRepository, _projectRepository, _projectService,
            _criticalPathService);

        _project = new Project()
        {
            Id = 35,
            Name = "Test Project",
            Description = "Description",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>()
        };
        _projectRepository.Add(_project);

        var resourceType = new ResourceType()
        {
            Name = "TaskServiceTestType"
        };
        _resourceTypeRepository.Add(resourceType);

        _resource = new Resource()
        {
            Name = "Resource",
            Description = "Description",
            Quantity = 1,
            Type = resourceType
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
    public void GetTaskDatesFromDto_ShouldCalculateCorrectEarlyStartAndFinishDates()
    {
        _task.EarlyStart = _project.StartDate.ToDateTime(new TimeOnly(0, 0));
        _task.EarlyFinish = _task.EarlyStart.AddDays(5);

        _project.Tasks = new List<Task> { _task };

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
        Assert.AreEqual(expectedProjectStart, startNoDeps,
            "Task without dependencies should start at project start date");
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

        Assert.AreEqual(_task.EarlyFinish.AddDays(1), startWithDeps,
            "Task with dependencies should start day after dependency finishes");
        Assert.AreEqual(_task.EarlyFinish.AddDays(1).AddDays(2), finishWithDeps,
            "Task finish should be start + duration");

        var taskWithBadDeps = new TaskDataDTO
        {
            Title = "Bad Dependencies Task",
            Description = "Task with non-existent dependencies",
            Duration = 1,
            Dependencies = new List<string> { "NonExistentTask" },
            Resources = new List<TaskResourceDataDTO>()
        };

        var (startBadDeps, finishBadDeps) = _taskService.GetTaskDatesFromDto(taskWithBadDeps, _project.Id);

        Assert.AreEqual(expectedProjectStart, startBadDeps,
            "Task with invalid dependencies should start at project start");
        Assert.AreEqual(expectedProjectStart.AddDays(1), finishBadDeps,
            "Task finish should be project start + duration");

        var exception = Assert.ThrowsException<ArgumentException>(() =>
            _taskService.GetTaskDatesFromDto(taskWithoutDeps, 999));

        Assert.AreEqual("Project with ID 999 not found", exception.Message,
            "Should throw exception with correct message");
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
                new TaskResourceDataDTO
                {
                    TaskTitle = "Task With Real Dependencies And Resources", ResourceId = existingResource.Id,
                    Quantity = 2
                }
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
                new TaskResourceDataDTO { TaskTitle = "NonexistentDep1", ResourceId = 99999, Quantity = 1 },
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
            Title = "Task With Completed Dependencies"
        };

        TaskDependency dep1 = new TaskDependency
        {
            Id = 1,
            Task = taskWithCompletedDependencies,
            Dependency = dependency1
        };

        TaskDependency dep2 = new TaskDependency
        {
            Id = 2,
            Task = taskWithCompletedDependencies,
            Dependency = dependency2
        };

        taskWithCompletedDependencies.Dependencies = new List<TaskDependency> { dep1, dep2 };
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
            Title = "Task With Mixed Dependencies"
        };

        TaskDependency dep1 = new TaskDependency
        {
            Id = 1,
            Task = taskWithMixedDependencies,
            Dependency = completedDependency
        };

        TaskDependency dep2 = new TaskDependency
        {
            Id = 2,
            Task = taskWithMixedDependencies,
            Dependency = pendingDependency
        };

        taskWithMixedDependencies.Dependencies = new List<TaskDependency> { dep1, dep2 };
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
            Title = "MainTask"
        };

        TaskDependency dep1 = new TaskDependency
        {
            Id = 1,
            Task = taskWithDependency,
            Dependency = dependencyTask1
        };

        taskWithDependency.Dependencies = new List<TaskDependency> { dep1 };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask"
        };

        TaskDependency dep2 = new TaskDependency
        {
            Id = 2,
            Task = taskWithoutSearchedDependency,
            Dependency = dependencyTask2
        };

        taskWithoutSearchedDependency.Dependencies = new List<TaskDependency> { dep2 };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<TaskDependency> dependencies =
            _taskService.GetTaskDependenciesWithTitleTask(searchList, taskWithDependency);

        Assert.AreEqual(1, dependencies.Count);
        Assert.AreEqual("Task1", dependencies[0].Dependency.Title);
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
            Title = "MainTask"
        };

        TaskDependency dep1 = new TaskDependency
        {
            Id = 1,
            Task = taskWithDependency,
            Dependency = dependencyTask1
        };

        taskWithDependency.Dependencies = new List<TaskDependency> { dep1 };
        _taskRepository.Add(taskWithDependency);

        Task taskWithoutSearchedDependency = new Task
        {
            Title = "AnotherTask"
        };

        TaskDependency dep2 = new TaskDependency
        {
            Id = 2,
            Task = taskWithoutSearchedDependency,
            Dependency = dependencyTask2
        };

        taskWithoutSearchedDependency.Dependencies = new List<TaskDependency> { dep2 };
        _taskRepository.Add(taskWithoutSearchedDependency);

        List<string> searchList = new List<string> { "Task1" };

        List<TaskDependency> dependencies =
            _taskService.GetTaskDependenciesWithTitleTask(searchList, taskWithDependency);

        Assert.AreEqual(1, dependencies.Count);
        Assert.AreEqual("Task1", dependencies[0].Dependency.Title);
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

    [TestMethod]
    public void GetTasksForProjectWithIdTest()
    {
        Task task1 = new Task() { Title = "Task 1" };
        Task task2 = new Task() { Title = "Task 2" };

        _taskRepository.Add(task1);
        _taskRepository.Add(task2);

        Project project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Tasks = new List<Task> { task1, task2 }
        };

        _projectRepository.Add(project);

        List<GetTaskDTO> tasks = _taskService.GetTasksForProjectWithId(1);

        Assert.AreEqual(2, tasks.Count);
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnTrueForCriticalTask_AndFalseForNonCriticalTask()
    {
        Task taskA = new Task { Title = "A", Duration = 2 };
        Task taskB = new Task { Title = "B", Duration = 3 };
        Task taskC = new Task { Title = "C", Duration = 1 };
        Task taskD = new Task { Title = "D", Duration = 2 };

        TaskDependency depB = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        TaskDependency depC = new TaskDependency
        {
            Id = 2,
            Task = taskC,
            Dependency = taskA
        };

        TaskDependency depD1 = new TaskDependency
        {
            Id = 3,
            Task = taskD,
            Dependency = taskB
        };

        TaskDependency depD2 = new TaskDependency
        {
            Id = 4,
            Task = taskD,
            Dependency = taskC
        };

        taskB.Dependencies = new List<TaskDependency> { depB };
        taskC.Dependencies = new List<TaskDependency> { depC };
        taskD.Dependencies = new List<TaskDependency> { depD1, depD2 };

        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task> { taskA, taskB, taskC, taskD }
        };

        _criticalPathService.CalculateEarlyTimes(project);
        _criticalPathService.CalculateLateTimes(project);

        Assert.IsTrue(_taskService.IsTaskCritical(project, taskA.Title));
        Assert.IsTrue(_taskService.IsTaskCritical(project, taskB.Title));
        Assert.IsTrue(_taskService.IsTaskCritical(project, taskD.Title));
        Assert.IsFalse(_taskService.IsTaskCritical(project, taskC.Title));
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        Project project = new Project
        {
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Tasks = new List<Task>()
        };

        bool result = _taskService.IsTaskCritical(project, "TareaInexistente");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsTaskCritical_ShouldReturnFalse_WhenProjectIsNull()
    {
        bool result = _taskService.IsTaskCritical(null, "AnyTask");
        Assert.IsFalse(result);
    }


    [TestMethod]
    public void RecalculateTaskDates_ShouldRecalculateDatesForAllTasks()
    {
        Project project = new Project
        {
            Id = 400,
            Name = "Recalculate Test Project",
            Description = "Test recalculation",
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            ProjectRoles = new List<ProjectRole>(),
            Tasks = new List<Task>()
        };

        Task taskA = new Task
        {
            Title = "Task A",
            Description = "First task",
            Duration = 3,
            Dependencies = new List<TaskDependency>(),
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue
        };

        Task taskB = new Task
        {
            Title = "Task B",
            Description = "Second task",
            Duration = 2,
            EarlyStart = DateTime.MinValue,
            EarlyFinish = DateTime.MinValue
        };

        TaskDependency dependency = new TaskDependency
        {
            Id = 1,
            Task = taskB,
            Dependency = taskA
        };

        taskB.Dependencies = new List<TaskDependency> { dependency };

        project.Tasks.Add(taskA);
        project.Tasks.Add(taskB);
        _projectRepository.Add(project);

        _taskService.RecalculateTaskDates(400);

        DateTime expectedStartA = project.StartDate.ToDateTime(new TimeOnly(0, 0));
        DateTime expectedFinishA = expectedStartA.AddDays(3);
        DateTime expectedStartB = expectedFinishA.AddDays(1);
        DateTime expectedFinishB = expectedStartB.AddDays(2);

        Assert.AreEqual(expectedStartA, taskA.EarlyStart);
        Assert.AreEqual(expectedFinishA, taskA.EarlyFinish);
        Assert.AreEqual(expectedStartB, taskB.EarlyStart);
        Assert.AreEqual(expectedFinishB, taskB.EarlyFinish);
    }

    [TestMethod]
    public void RecalculateTaskDates_ShouldDoNothing_WhenProjectNotFound()
    {
        _taskService.RecalculateTaskDates(999);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void DependsOnTasksFromAnotherProject_ShouldReturnTrue_WhenTaskDependsOnExternalProjectTask()
    {
        Project project1 = new Project
        {
            Id = 1,
            Name = "Project 1",
            Tasks = new List<Task>()
        };

        Project project2 = new Project
        {
            Id = 2,
            Name = "Project 2",
            Tasks = new List<Task>()
        };

        Task externalTask = new Task { Title = "External Task" };
        Task task = new Task { Title = "Task" };

        TaskDependency dependency = new TaskDependency
        {
            Id = 1,
            Task = task,
            Dependency = externalTask
        };

        task.Dependencies = new List<TaskDependency> { dependency };

        project1.Tasks.Add(task);
        project2.Tasks.Add(externalTask);

        _projectRepository.Add(project1);
        _projectRepository.Add(project2);
        _taskRepository.Add(task);
        _taskRepository.Add(externalTask);

        bool result = _taskService.DependsOnTasksFromAnotherProject("Task", 1);

        Assert.IsTrue(result, "Task should depend on tasks from another project");
    }


    [TestMethod]
    public void DependsOnTasksFromAnotherProject_ShouldReturnFalse_WhenTaskDependsOnSameProjectTasks()
    {
        Project project = new Project
        {
            Id = 1,
            Name = "Project",
            Tasks = new List<Task>()
        };

        Task dependencyTask = new Task { Title = "Dependency Task" };
        Task task = new Task { Title = "Task" };

        TaskDependency dependency = new TaskDependency
        {
            Id = 1,
            Task = task,
            Dependency = dependencyTask
        };

        task.Dependencies = new List<TaskDependency> { dependency };

        project.Tasks.Add(task);
        project.Tasks.Add(dependencyTask);

        _projectRepository.Add(project);
        _taskRepository.Add(task);
        _taskRepository.Add(dependencyTask);

        bool result = _taskService.DependsOnTasksFromAnotherProject("Task", 1);

        Assert.IsFalse(result, "Task should not depend on tasks from another project");
    }
}