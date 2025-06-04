namespace DTOs.UserDTOs;

public class UserDataDTO
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.Today;
    public string Password { get; set; } = string.Empty;
    public bool Admin { get; set; } = false;
    
}