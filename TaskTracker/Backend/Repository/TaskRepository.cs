namespace Backend.Repository;
using Backend.Domain;

public class TaskRepository : IRepository<Backend.Domain.Task>
{
    private readonly List<Backend.Domain.Task> _taskRepository;

    public TaskRepository()
    {
        _taskRepository = new List<Backend.Domain.Task>();
    }

    public Backend.Domain.Task Add(Backend.Domain.Task task)
    {
        throw new NotImplementedException();
    }

    public Backend.Domain.Task? Find(Func<Backend.Domain.Task, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public IList<Backend.Domain.Task> FindAll()
    {
        throw new NotImplementedException();
    }

    public Backend.Domain.Task? Update(Backend.Domain.Task entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}