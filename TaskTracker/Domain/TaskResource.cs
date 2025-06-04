namespace Domain;

public class TaskResource()
{
    private Task _task;
    private Resource _resource;
    private int _quantity;

    public Resource Resource
    {
        get => _resource;
        set => _resource = value;
    }
    public Task Task
    { 
        get => _task;
        set
        {
            _task = value;
        }
    }
    public int Quantity 
    { 
        get => _quantity; 
        set
        {
            if (value < 0) throw new ArgumentException("Quantity cannot be negative");
            _quantity = value;
        }
    }
}