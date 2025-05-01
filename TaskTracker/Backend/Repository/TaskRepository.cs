namespace Backend.Repository;

public class TaskRepository : IRepository<Task>
{
    private readonly List<Task> _taskRepository;

    public TaskRepository()
    {
        _taskRepository = new List<Task>();
    }

    public Task Add(Task entity)
    {
        throw new NotImplementedException();
    }

    public Task? Find(Func<Task, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public IList<Task> FindAll()
    {
        throw new NotImplementedException();
    }

    public Task? Update(Task entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}