using Backend.Domain;

namespace Backend.DTOs;

public class CreateUserDTOs
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.Today;
    public string Password { get; set; } = string.Empty;
    public bool Admin { get; set; } = false;
    
    public User ToEntity()
    {
        return new User()
        {
            Name = Name,
            LastName = LastName,
            Email = Email,
            BirthDate = BirthDate,
            Password = Password,
            Admin = Admin
        };
    }
}