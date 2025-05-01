namespace Backend.DTOs;

public class CreateUserDTOs
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.Today;
    public string Password { get; set; } = string.Empty;
}