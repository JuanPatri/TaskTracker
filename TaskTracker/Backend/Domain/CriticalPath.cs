namespace Backend.Domain;

public class CriticalPath
{
    private Project _project;
    private List<ProjectTask> _criticalPathTasks;
    
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

    public List<ProjectTask> CriticalPathTasks
    {
        get => _criticalPathTasks;
        set => _criticalPathTasks = value;
        
    }

}