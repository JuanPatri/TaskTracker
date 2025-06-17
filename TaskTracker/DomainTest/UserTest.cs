using Domain;
using DTOs.UserDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;
using Service;

namespace DomainTest;

[TestClass]
public class UserTest
{
    private User _user;
    private UserService _userService;
    private UserRepository _userRepository;
    private SqlContext _sqlContext;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _user = new User();
        _userRepository = new UserRepository(_sqlContext);
        _userService = new UserService(_userRepository);
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(_user);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        _user.Name = "Pedro";
        Assert.AreEqual("Pedro", _user.Name);
    }
    
    [TestMethod]
    public void CreateProjectsListForUser()
    {
        _user.Projects = new List<Project>();
        Assert.IsNotNull(_user.Projects);
    }
    
    [TestMethod]
    public void UserHasAProjectsList()
    {
        Assert.IsNotNull(_user.Projects);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutNameNullReturnsAnException()
    {
        _user.Name = null;
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        _user.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", _user.LastName);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutLastNameNullReturnsAnException()
    {
        _user.LastName = null;
    }
    
    [TestMethod]
    public void AddEmailToUser()
    {
        _user.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", _user.Email);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutEmailNullReturnsAnException()
    {
        _user.Email = null;
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DontPutAnArrobaInEmail()
    {
        _user.Email  = "prodriguezgmail.com";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void DontPutAnPointInEmail()
    {
        _user.Email  = "prodriguez@gmailcom";
    }
    [TestMethod]
    public void AddBirthDateToUser()
    {
        DateTime fecha = new DateTime(2003, 03, 14);
        _user.BirthDate = fecha;
        Assert.AreEqual(fecha, _user.BirthDate);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutBirthDateNullReturnsAnException()
    {
        _user.BirthDate = default;
    }

    [TestMethod]
    public void AddPasswordToUser()
    {
        _user.Password = "Pedro1234@";
        Assert.AreEqual("Pedro1234@", _user.Password);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordNullReturnsAnException()
    {
        _user.Password = null;
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordWithoutUpperCaseReturnsAnException()
    {
        _user.Password = "pedro1234@";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordWithoutLowerCaseReturnsAnException()
    {
         _user.Password = "PEDRO1234@";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutNameWithNumbersReturnsAnException()
    {
        _user.Name = "Pedro1234";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordWithoutSpecialCharacterReturnsAnException()
    {
        _user.Password = "Pedro12345";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutLastNameWithNumbersReturnsAnException()
    {
        _user.LastName = "Rodriguez1234";
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BirthDate_SetWithAgeLessThan18_ThrowsArgumentException()
    {
        _user.BirthDate = new DateTime(2025, 03, 14);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BirthDate_SetWithAgeGreaterThan100_ThrowsArgumentException()
    {
        _user.BirthDate = new DateTime(1900, 03, 14);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordWithoutNumberReturnsAnException()
    {
        _user.Password = "PedroRodriguez@";
    }
    
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void PutPasswordLessThan8CharactersReturnsAnException()
    {
        _user.Password = "Pe1@";
    }

    [TestMethod]
    public void PutAdminInFalse()
    {
        _user.Admin = false;
        Assert.IsFalse(_user.Admin);
    }

    [TestMethod]
    public void UserToEntityReturnUser()
    {
        UserDataDTO userDataDto = new UserDataDTO()
        {
            Name = "Pedro",
            LastName = "Rodriguez",
            Email = "prodriguez@gmail.com",
            BirthDate = new DateTime(2003, 03, 14),
            Password = "Pedro1234@",
            Admin = false
        };
        
        User user = _userService.FromDto(userDataDto);
        
        Assert.AreEqual(userDataDto.Name, user.Name);
    }
    
}