using DTOs.SessionDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTOsTest.SessionDTOsTest;

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