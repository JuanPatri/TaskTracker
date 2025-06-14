using Domain;
using DTOs.SessionDTOs;
using DTOs.UserDTOs;

namespace Service;

public class SessionService
{
    private readonly UserService _userService;
    
    private static User? _currentUser;
    
    public User? CurrentUser => _currentUser;
    
    public SessionService(UserService userService)
    {
        _userService = userService;
    }
    
    public void Login(LoginDto loginDto)
    {
        GetUserDTO userDto = new GetUserDTO()
        {
            Email = loginDto.Email
        };
        User? user = _userService.GetUser(userDto);
        
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.Password != loginDto.Password)
        {
            throw new ArgumentException("Invalid password");
        }
        
        _currentUser = user; 
    }

    public void Logout()
    {
        _currentUser = null;
    }

    public bool IsLoggedIn()
    {
        return _currentUser != null;
    }
}