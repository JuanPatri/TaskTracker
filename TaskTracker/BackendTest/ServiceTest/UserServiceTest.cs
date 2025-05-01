using Backend.Repository;

namespace BackendTest.ServiceTest;
using Backend.Service;

[TestClass]
public class UserServiceTest
{
    private UserService _userService;
    private UserRepository _userRepository;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _userRepository = new UserRepository();
        _userService = new UserService(_userRepository);
    }
    
    [TestMethod]
    public void CreateUserService()
    {
        Assert.IsNotNull(_userService);
    }
    
    
}