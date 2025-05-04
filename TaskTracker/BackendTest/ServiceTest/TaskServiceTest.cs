using Backend.Domain;
using Backend.DTOs.TaskDTOs;
using Backend.Service;
using Backend.Domain.Enums;
using Backend.Repository;
using Task = Backend.Domain.Task;

namespace BackendTest.ServiceTest;

[TestClass]
public class TaskServiceTest
{

    private TaskRepository _taskRepository;
    private ProjectRepository _projectRepository;
    private ResourceRepository _resourceRepository;
    private TaskService _taskService;
    private Task _task;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _projectRepository = new ProjectRepository();
        _resourceRepository = new ResourceRepository();
        _taskService = new TaskService(_taskRepository, _projectRepository, _resourceRepository);
    
        Project project = new Project() { Id = 35, Name = "Test Project" };
        _projectRepository.Add(project);

        _task = new Task() { Title = "Test Task", };
        _taskRepository.Add(_task);
    }

    [TestMethod]
    public void AddTaskToRepository()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        taskDto.Description = "This is a test task.";
        taskDto.Duration = TimeSpan.FromHours(1);
        taskDto.Status = Status.Pending;
        taskDto.Project = "35"; 
        taskDto.Dependencies = new List<string>(){"Task1", "Task2"};
        taskDto.Resources = new List<(int, string)>(){(1, "Resource1"), (2, "Resource2")};

        Task? task = _taskService.AddTask(taskDto);
    
        Assert.IsNotNull(task);
        Assert.AreEqual(_taskRepository.FindAll().Last(), task);
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
        task2.Duration = TimeSpan.FromHours(1);
        task2.Status = Status.Pending;
        _taskService.AddTask(task2);
        tasks = _taskService.GetAllTasks();
        
        Assert.AreEqual(2, tasks.Count);
    }
    
    [TestMethod]
    public void UpdateTaskShouldUpdateTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
        taskDto.Description = "New Description";
        taskDto.Duration = TimeSpan.FromHours(4);
        taskDto.Status = Status.Blocked;
        taskDto.Dependencies = new List<string>(){"Task1", "Task2"};
        
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
    public void RemoveTaskShouldDeleteTask()
    {
        Assert.AreEqual(1, _taskRepository.FindAll().Count());
        
        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test Task";
        _taskService.RemoveTask(taskDto);
        
        Assert.AreEqual(0, _taskRepository.FindAll().Count());
    }
}