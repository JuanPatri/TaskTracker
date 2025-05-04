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

    public TaskService(IRepository<Task> taskRepository, IRepository<Project> projectRepository,
        IRepository<Resource> resourceRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _resourceRepository = resourceRepository;
    }

    public List<Task> GetTaskDependenciesWithTitle(List<string> titlesTask)
    {
        List<Task> taskDependencies = _taskRepository.FindAll()
            .Where(t => t.Dependencies.Any(dependency => titlesTask.Contains(dependency.Title)))
            .ToList();

        return taskDependencies;
    }

    public List<(int, Resource)> GetResourcesWithName(List<(int, string)> namesResource)
    {
        List<(int, Resource)> resourceList = namesResource
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

        return resourceList;
    }

    public Task AddTask(TaskDataDTO task)
    {

        Project project = _projectRepository.Find(p => p.Id == int.Parse(task.Project));;

        List<Task> taskDependencies = GetTaskDependenciesWithTitle(task.Dependencies);

        List<(int, Resource)> resourceList = GetResourcesWithName(task.Resources);

        Task createdTask = task.ToEntity(taskDependencies, project, resourceList);
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
        
        Project? project = _projectRepository.Find(p => p.Id == int.Parse(taskDto.Project));
        
        List<(int, Resource)> resourceList = taskDto.Resources
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

        return _taskRepository.Update(taskDto.ToEntity(taskDependencies, project, resourceList));
    }
    public void RemoveTask(GetTaskDTO task)
    {
        _taskRepository.Delete(task.Title);
    }
}