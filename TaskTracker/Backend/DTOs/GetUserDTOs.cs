using Backend.Domain;

namespace Backend.DTOs;

public class GetUserDTOs
{
    public string Email { get; set; } = string.Empty;
    
    public User ToEntity()
    {
        return new User() 
        {
            Email = Email
        };
    }
}