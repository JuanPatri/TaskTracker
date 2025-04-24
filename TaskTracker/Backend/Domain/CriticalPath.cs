namespace Backend.Domain;

public class CriticalPath
{
    private Project _project;
    
    public Project Project
    {
        get => _project;
        set
        {
            _project = value;
        }
    }

}