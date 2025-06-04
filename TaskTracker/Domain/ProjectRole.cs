using Domain.Enums;

namespace Domain;
public class ProjectRole
{
    private RoleType _roleType;
    private Project _project;
    private User _user;
    public RoleType RoleType
    {
        get => _roleType;
        set =>  _roleType = value;
    }

    public Project Project
    {
        get => _project;
        set {
            if(value == null) 
                throw new ArgumentException("Project cannot be null"); 
            
            _project = value;
    } 
    }
    public User User
    {
        get => _user;
        set => _user = value;
        
    }
    
}