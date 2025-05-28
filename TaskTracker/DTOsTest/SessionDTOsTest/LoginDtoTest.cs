using Backend.Domain;
using Backend.DTOs.SessionDTOs;

namespace BackendTest.DTOsTest.SessionDTOsTest;

[TestClass]
public class LoginDtoTest
{
    
    [TestMethod]
    public void LoginDtoShouldHaveDefaultValues()
    {
        LoginDto loginDto = new LoginDto();

        
        Assert.AreEqual(string.Empty, loginDto.Email);
        Assert.AreEqual(string.Empty, loginDto.Password);
    }
    
}