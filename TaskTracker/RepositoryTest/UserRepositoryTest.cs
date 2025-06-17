using Domain;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;

namespace BackendTest.RepositoryTest;

[TestClass]
public class UserRepositoryTest
{
    private UserRepository _userRepository;
    private User _user;
    private SqlContext _sqlContext;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _userRepository = new UserRepository(_sqlContext);
        _user = new User
        {
            Email = "usuario@nuevo.com",
            Name = "Test Name",           
            LastName = "Test LastName",    
            Password = "Password123!",    
            BirthDate = new DateTime(1990, 1, 1),
            Admin = false
        };
    }
    
    [TestMethod]
    public void CreateUserRepository()
    { 
        Assert.IsNotNull(_userRepository);
    }

    [TestMethod]
    public void AddUserToList()
    {
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.Find(u => u.Email == "usuario@nuevo.com"), _user);
    }

    [TestMethod]
    public void SearchForAllUserInTheList()
    {
        Assert.AreEqual(_userRepository.FindAll().Count, 1);
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.FindAll().Count, 2);
    }

    [TestMethod]
    public void UpdateExistingUserUpdatesFieldsCorrectly()
    {
        _userRepository.Add(_user);
        
        User updateUser = new User()
        {
            Name = "UpdatedName",
            LastName = "UpdatedLastName",
            Password = "UpdatedPassword123!",
            Admin = false,
            BirthDate = new DateTime(1995, 5, 5),
            Email = "usuario@nuevo.com"
        };
        User updatedUser = _userRepository.Update(updateUser);
        Assert.IsNotNull(updatedUser);
        Assert.AreEqual("UpdatedName", updatedUser.Name);
    }

    [TestMethod]
    public void UpdatingAUserThatIsNotInTheListReturnsNull()
    {
        _user.Name = "Name";
        _userRepository.Add(_user);
        User updateUser = new User()
        {
            Name = "UpdatedName",
            LastName = "UpdatedLastName",
            Password = "UpdatedPassword123!",
            Admin = false,
            BirthDate = new DateTime(1995, 5, 5),
            Email = "Email@incorrecto.com"
        };
        _userRepository.Update(updateUser);
        Assert.AreEqual(_userRepository.Update(updateUser), null);
    }
    
    [TestMethod]
    public void DeleteUserFromList()
    {
        _userRepository.Add(_user);
        Assert.AreEqual(_userRepository.FindAll().Count, 2);
        _userRepository.Delete(_user.Email);
        Assert.AreEqual(_userRepository.FindAll().Count, 1);
    }
}