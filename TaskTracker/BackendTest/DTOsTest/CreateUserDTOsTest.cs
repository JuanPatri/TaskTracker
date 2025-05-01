using Backend.Domain;
using Backend.DTOs;

namespace BackendTest.DTOsTest;

[TestClass]
public class CreateUserDTOsTest
{
    [TestMethod]
    public void CreateNameForUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Name = "Pedro";
        Assert.AreEqual("Pedro", userDto.Name);
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", userDto.LastName);
    }
    
    [TestMethod]
    public void AddEmailToUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDto.Email);
    }
    
    [TestMethod]
    public void AddBirthDateToUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        DateTime date = new DateTime(2003, 03, 14);
        userDto.BirthDate = date;
        Assert.AreEqual(date, userDto.BirthDate);
    }
    
    [TestMethod]
    public void AddPasswordToUser()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Password = "Pedro1234@";
        Assert.AreEqual("Pedro1234@", userDto.Password);
    }
    
    [TestMethod]
    public void SetAdminInFalse()
    {
        CreateUserDTOs userDto = new CreateUserDTOs();
        userDto.Admin = false;
        Assert.IsFalse(userDto.Admin);
    }

    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    {
        CreateUserDTOs dto = new CreateUserDTOs
        {
            Name = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            BirthDate = new DateTime(1995, 5, 20),
            Password = "Secure123!",
            Admin = true
        };
        
        User user = dto.ToEntity();

        Assert.IsNotNull(user);
    }
}