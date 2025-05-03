using Backend.DTOs.TaskDTOs;
using Backend.Service;
using Backend.Domain.Enums;
using Backend.Repository;
using Task = Backend.Domain.Task;

namespace BackendTest.ServiceTest;

public class TaskServiceTest
{
    [TestMethod]
    public void AddTaskToRepository()
    {
        TaskRepository _taskRepository = new TaskRepository();
        TaskService _taskService = new TaskService(_taskRepository);
        
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
}