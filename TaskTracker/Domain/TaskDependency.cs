namespace Domain;

public class TaskDependency
{
    private int _id;
    private Task _task;
    private Task _dependency;
    
    public int Id
    {
        get => _id;
        set => _id = value;
    }
    
    public Task Task
    {
        get => _task;
        set
        {
            if (value == null) 
                throw new ArgumentException("Task cannot be null");
            _task = value;
        }
    }
    
    public Task Dependency
    {
        get => _dependency;
        set
        {
            if (value == null) 
                throw new ArgumentException("Dependency cannot be null");
            _dependency = value;
        }
    }
}