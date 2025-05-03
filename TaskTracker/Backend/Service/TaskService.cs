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
    
    public Task GetTaskByTitle(string title)
    {
        return _taskRepository.Find(t => t.Title == title);
    }

    // Hacer en service task
    // List<Task> taskDependencies = new List<Task>();
    // for(int i = 0; i < Dependencies.Count; i++)
    // {
    //     taskDependencies.Add(FindAll().Find(t => t.title == Dependencies[i])); 
    // }
}