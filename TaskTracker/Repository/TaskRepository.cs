using Enums;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Task;

namespace Repository;

public class TaskRepository : IRepository<Task>
{
    private readonly SqlContext _sqlContext;

    public TaskRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;
    }

    public Task Add(Task task)
    {
        _sqlContext.Tasks.Add(task);
        _sqlContext.SaveChanges();
        return task;
    }

    public Task? Find(Func<Task, bool> predicate)
    {
        return _sqlContext.Tasks
            .Include(t => t.Resources)
            .FirstOrDefault(predicate);
    }

    public IList<Task> FindAll()
    {
        return _sqlContext.Tasks.Include(t => t.Resources).ToList();
    }

    public Task? Update(Task updateTask)
    {
        Task? existingTask = _sqlContext.Tasks.FirstOrDefault(task => task.Title == updateTask.Title);
        if (existingTask != null)
        {
            existingTask.Description = updateTask.Description;
            existingTask.Duration = updateTask.Duration;
            existingTask.Status = updateTask.Status;
        
            if(existingTask.Status == Status.Completed)
            {
                existingTask.DateCompleated = DateTime.Now;
            }
        
            _sqlContext.SaveChanges(); 
            return existingTask;
        }

        return null;
    }

    public void Delete(string title)
    {
        Task? taskToDelete = _sqlContext.Tasks.FirstOrDefault(task => task.Title == title);
        if (taskToDelete != null)
        {
            _sqlContext.Tasks.Remove(taskToDelete);
            _sqlContext.SaveChanges();
        }
    }
    
}