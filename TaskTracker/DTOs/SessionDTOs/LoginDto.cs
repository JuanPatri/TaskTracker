using Backend.Domain;

namespace Backend.DTOs.SessionDTOs;
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}