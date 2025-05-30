namespace Domain;

public class TaskResource
{
    private int _taskId;
    private int _resourceId;
    private int _quantity;
    
    public Resource Resource { get; set; }
    public int TaskId 
    { 
        get => _taskId;
        set
        {
            _taskId = value;
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
            if (value <= 0) throw new ArgumentException("Quantity must be greater than 0");
            _quantity = value;
        }
    }
}