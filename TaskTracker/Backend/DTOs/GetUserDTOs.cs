namespace Backend.DTOs;

public class GetUserDTOs
{
    public string Email { get; set; } = string.Empty;
    
    public string ToEntity()
    {
        return Email;
    }
}