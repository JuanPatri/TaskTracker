namespace Domain;

public class TaskResource
{
    private int _taskId;
    private int _resourceId;
    
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
    
}