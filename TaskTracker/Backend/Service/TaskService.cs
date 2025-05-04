using Backend.Domain;
using Backend.DTOs.TaskDTOs;
using Backend.Repository;
using Task = Backend.Domain.Task;

namespace Backend.Service;

public class TaskService
{
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<Resource> _resourceRepository;
    public  TaskService(IRepository<Task> taskRepository, IRepository<Project> projectRepository, IRepository<Resource> resourceRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
    }
    
    public Task AddTask(TaskDataDTO task)
    {
        Project? project = _projectRepository.Find(p => p.Id == task.Project);
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => task.Dependencies.Contains(t.Title))
            .ToList();
        
        List<(int, Resource)> resourceList = task.Resources
            .Select(tuple =>
            {
                int cantidad = tuple.Item1;
                string nameResource = tuple.Item2;

                Resource? resource = _resourceRepository.Find(r => r.Name == nameResource);
                return (cantidad, resource);
            })
            .Where(t => t.Item2 != null)
            .Select(t => (t.Item1, t.Item2!)) 
            .ToList();

        Task createdTask = task.ToEntity(taskDependencies, project);
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
        
        Project? project = _projectRepository.Find(p => p.Id == taskDto.Project);
        return _taskRepository.Update(taskDto.ToEntity(taskDependencies, project));
    }
    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);
    }
}