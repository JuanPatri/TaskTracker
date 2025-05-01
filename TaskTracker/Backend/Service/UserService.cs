using Backend.Domain;
using Backend.Repository;

namespace Backend.Service;

public class UserService
{
    private readonly IRepository<User> _userRepository;
    public  UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }
}