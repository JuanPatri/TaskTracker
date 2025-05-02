using Backend.Domain;
using Backend.DTOs;

namespace BackendTest.DTOsTest;

[TestClass]
public class UserDataDtoTest
{
    [TestMethod]
    public void CreateNameForUser()
    {
        UserDataDTO userDto = new UserDataDTO();
        userDto.Name = "Pedro";
        Assert.AreEqual("Pedro", userDto.Name);
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        UserDataDTO userDto = new UserDataDTO();
        userDto.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", userDto.LastName);
    }
    
    [TestMethod]
    public void AddEmailToUser()
    {
        UserDataDTO userDto = new UserDataDTO();
        userDto.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", userDto.Email);
    }
    
    [TestMethod]
    public void AddBirthDateToUser()
    {
        UserDataDTO userDto = new UserDataDTO();
        DateTime date = new DateTime(2003, 03, 14);
        userDto.BirthDate = date;
        Assert.AreEqual(date, userDto.BirthDate);
    }
    
    [TestMethod]
    public void AddPasswordToUser()
    {
        UserDataDTO userDto = new UserDataDTO();
        userDto.Password = "Pedro1234@";
        Assert.AreEqual("Pedro1234@", userDto.Password);
    }
    
    [TestMethod]
    public void SetAdminInFalse()
    {
        UserDataDTO userDto = new UserDataDTO();
        userDto.Admin = false;
        Assert.IsFalse(userDto.Admin);
    }

    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    {
        UserDataDTO dto = new UserDataDTO
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