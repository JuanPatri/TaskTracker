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
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => task.Dependencies.Contains(t.Title))
            .ToList();
        
        Task createdTask = task.ToEntity(taskDependencies);
        return _taskRepository.Add(createdTask);
    }
    
    public Task GetTaskByTitle(string title)
    {
        return _taskRepository.Find(t => t.Title == title);
    }

    public List<Task> GetAllTasks()
    {
        return _taskRepository.FindAll().ToList();
    }

    public Task? UpdateTask(TaskDataDTO taskDto)
    {
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => taskDto.Dependencies.Contains(t.Title))
            .ToList();   
        return _taskRepository.Update(taskDto.ToEntity(taskDependencies));
    }
    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);
    }
}