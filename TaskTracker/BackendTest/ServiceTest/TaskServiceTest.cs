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
    private TaskService _taskService;
    private Task _task;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _taskService = new TaskService(_taskRepository);
        _task = new Task()
        {
            Title = "Test Task",
        };
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
        // taskDto.Project = 0;
        // taskDto.Dependencies = new List<string>(){"Task1", "Task2"};

        Task? task = _taskService.AddTask(taskDto);
        
        Assert.IsNotNull(task);
        Assert.AreEqual(_taskRepository.FindAll().Last(), task);
    }

    [TestMethod]
    public void FindTaskByTitleReturnTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Test Task";
    }
}