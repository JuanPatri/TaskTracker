namespace Backend.Domain;
using Enums;

public class ProjectRole
{
    private RoleType _roleType;
    private List<Project> _project;
    public RoleType RoleType
    {
        get => _roleType;
        set =>  _roleType = value;
    }

    public List<Project> Project
    {
        get => _project;
        set => _project = value;
        
    }
    
}