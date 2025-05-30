namespace Domain;

public class TaskResource
{
    private int _taskId;
    
    public int TaskId 
    { 
        get => _taskId;
        set
        {
            _taskId = value;
        }
    }
}