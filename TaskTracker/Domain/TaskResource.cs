namespace Domain;

public class TaskResource()
{
    private Task _task;
    private int _resourceId;
    private int _quantity;
    
    public Resource Resource { get; set; }
    public Task Task
    { 
        get => _task;
        set
        {
            _task = value;
        }
    }
    
    public int ResourceId 
    { 
        get => _resourceId; 
        set
        {
            _resourceId = value;
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