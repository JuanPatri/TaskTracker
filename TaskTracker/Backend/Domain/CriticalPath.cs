namespace Backend.Domain;

public class CriticalPath
{
    private Project _project;
    
    public Project Project
    {
        get => _project;
        set
        {
           if (value == null)
               throw new ArgumentException("Project cannot be null");
           
               _project = value;
        } 
    }

}