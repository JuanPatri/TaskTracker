using Backend.Domain;
using Backend.DTOs.SessionDTOs;

namespace BackendTest.DTOsTest.SessionDTOsTest;

[TestClass]
public class LoginDtoTest
{
    
    [TestMethod]
    public void ToEntityShouldReturnUser()
    {
        LoginDto dto = new LoginDto
        {
            Email = "test@mail.com",
            Password = "Password123!"
        };
        
        User user = dto.ToEntity();
        
        Assert.AreEqual(dto.Email, user.Email);
        Assert.AreEqual(dto.Password, user.Password);
    }
}