using Backend.DTOs.TaskDTOs;
using Backend.Repository;
using Task = Backend.Domain.Task;

namespace Backend.Service;

public class TaskService
{
    private readonly IRepository<Task> _taskRepository;
    public  TaskService(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }
    
    public Task AddTask(TaskDataDTO task)
    {
        Task createdTask = task.ToEntity();
        return _taskRepository.Add(createdTask);
    }

}