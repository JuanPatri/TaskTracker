namespace Backend.Repository;
using Backend.Domain;

public class TaskRepository : IRepository<Task>
{
    private readonly List<Task> _taskRepository;

    public TaskRepository()
    {
        _taskRepository = new List<Task>();
    }

    public Task Add(Task task)
    {
        _taskRepository.Add(task);
        return task;
    }

    public Task? Find(Func<Task, bool> predicate)
    { 
        return _taskRepository.FirstOrDefault(predicate);;
    }

    public IList<Task> FindAll()
    {
        return _taskRepository;
    }

    public Task? Update(Task updateTask)
    {
        Task? existingTask = _taskRepository.FirstOrDefault(u => u.Title == updateTask.Title);
        if (existingTask != null)
        {
            existingTask.Description = updateTask.Description;
            existingTask.Date = updateTask.Date;
            existingTask.DurationTask = updateTask.DurationTask;
            existingTask.Status = updateTask.Status;
            existingTask.Project = updateTask.Project;
            existingTask.Dependencies = updateTask.Dependencies;
            
            return existingTask;
        }
        return null;
    }

    public void Delete(string title)
    {
        Task? taskToDelete = _taskRepository.FirstOrDefault(task => task.Title == title);
        if (taskToDelete != null)
        {
            _taskRepository.Remove(taskToDelete);
        }
    }
}