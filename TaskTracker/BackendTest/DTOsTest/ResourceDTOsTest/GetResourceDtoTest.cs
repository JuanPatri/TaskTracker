using Backend.Domain;
using Backend.DTOs.ResourceDTOs;

namespace BackendTest.DTOsTest.ResourceDTOsTest;

[TestClass]
public class GetResourceDtoTest
{
    [TestMethod]
    public void CreateNameForResource()
    {
        GetResourceDto resourceDto = new GetResourceDto();
        resourceDto.Name = "resource";
        Assert.AreEqual("resource", resourceDto.Name);
    }
    
    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    {
        GetResourceDto resourceDto = new GetResourceDto
        {
            Name = "ResourceName"
        }; 
        
        Resource resource = resourceDto.ToEntity();

        Assert.IsNotNull(resource);
    }
}