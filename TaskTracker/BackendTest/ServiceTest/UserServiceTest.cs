using System.Runtime.InteropServices.JavaScript;
using Backend.Domain;
using Backend.DTOs.UserDTOs;
using Backend.Repository;

namespace BackendTest.ServiceTest;
using Backend.Service;

[TestClass]
public class  UserServiceTest
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
        UserDataDTO user = new UserDataDTO()
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

        GetUserDTO userToDelete = new GetUserDTO()
        {
            Email = "pedro@gmail.com"
        };
        _userService.RemoveUser(userToDelete);
        
        Assert.AreNotEqual(_userRepository.FindAll().Last(), _user);
    }

    [TestMethod]
    public void GetUserReturnUser()
    {
        GetUserDTO userToFind = new GetUserDTO()
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

    [TestMethod]
    public void UpdateUserShouldModifyUserData()
    {
        _user.Name = "name";
        Assert.AreEqual(_user.Name, "name");

        UserDataDTO userDTO = new UserDataDTO()
        {
            Name = "Pedro",
            LastName = "Rodriguez",
            BirthDate = new DateTime(2003, 03, 14),
            Email = "pedro@gmail.com",
            Password = "Pedrito123!",
            Admin = false
        };
        
        _userService.UpdateUser(userDTO);
        Assert.AreEqual(_user.Name, "Pedro");
    }
}