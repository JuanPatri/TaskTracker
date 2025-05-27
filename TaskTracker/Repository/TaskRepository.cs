using Backend.Domain.Enums;

namespace Backend.Repository;
using Backend.Domain;

public class TaskRepository : IRepository<Task>
{
    private readonly List<Task> _tasks;

    public TaskRepository()
    {
        _tasks = new List<Task>();
    }

    public Task Add(Task task)
    {
        _tasks.Add(task);
        return task;
    }

    public Task? Find(Func<Task, bool> predicate)
    { 
        return _tasks.FirstOrDefault(predicate);;
    }

    public IList<Task> FindAll()
    {
        return _tasks;
    }

    public Task? Update(Task updateTask)
    {
        Task? existingTask = _tasks.FirstOrDefault(task => task.Title == updateTask.Title);
        if (existingTask != null)
        {
            existingTask.Description = updateTask.Description;
            existingTask.Duration = updateTask.Duration;
            existingTask.Status = updateTask.Status;
            if(existingTask.Status == Status.Completed)
            {
                existingTask.DateCompleated = DateTime.Now;
            }
            return existingTask;
        }

        return null;
    }

    public void Delete(string title)
    {
        Task? taskToDelete = _tasks.FirstOrDefault(task => task.Title == title);
        if (taskToDelete != null)
        {
            _tasks.Remove(taskToDelete);
        }
    }
    
}