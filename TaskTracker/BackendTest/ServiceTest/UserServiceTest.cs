using System.Runtime.InteropServices.JavaScript;
using Backend.Domain;
using Backend.DTOs;
using Backend.Repository;

namespace BackendTest.ServiceTest;
using Backend.Service;

[TestClass]
public class UserServiceTest
{
    private UserService _userService;
    private UserRepository _userRepository;
    private User _user;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _userRepository = new UserRepository();
        _userService = new UserService(_userRepository);
        
        _user = new User()
        {
            Email = "pedro@gmail.com"
        };
        _userRepository.Add(_user);
    }
    
    [TestMethod]
    public void CreateUserService()
    {
        Assert.IsNotNull(_userService);
    }

    [TestMethod]
    public void AddUserShouldReturnUser()
    {
        CreateUserDTOs user = new CreateUserDTOs()
        {
            Name = "Pedro",
            LastName = "Rodriguez",
            BirthDate = new DateTime(2003, 03, 14),
            Email = "user@user.com",
            Password = "Pedro1234@"
        };
        User? createdUser = _userService.AddUser(user);
        Assert.IsNotNull(createdUser);
        Assert.AreEqual(_userRepository.FindAll().Last(), createdUser);
    }

    [TestMethod]
    public void RemoveUserShouldRemoveUser()
    {
        Assert.AreEqual(_userRepository.FindAll().Last(), _user);   

        GetUserDTOs userToDelete = new GetUserDTOs()
        {
            Email = "pedro@gmail.com"
        };
        _userService.RemoveUser(userToDelete);
        
        Assert.AreNotEqual(_userRepository.FindAll().Last(), _user);
    }

    [TestMethod]
    public void GetUserReturnUser()
    {
        GetUserDTOs userToFind = new GetUserDTOs()
        {
            Email = "pedro@gmail.com"
        };
        
        Assert.AreEqual(_userService.GetUser(userToFind), _user);
    }

    [TestMethod]
    public void GetAllUsersReturnAllUsers()
    {
        List <User> users = _userService.GetAllUsers();
        
        Assert.IsTrue(users.Any(u => u.Email == "admin@admin.com"));
        Assert.IsTrue(users.Any(u => u.Email == "pedro@gmail.com"));
    }
}