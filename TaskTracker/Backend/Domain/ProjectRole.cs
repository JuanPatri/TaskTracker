namespace Backend.Domain;
using Enums;

public class ProjectRole
{
    private RoleType _roleType;

    public RoleType RoleType
    {
        get => _roleType;
        set =>  _roleType = value;
    }
    
}