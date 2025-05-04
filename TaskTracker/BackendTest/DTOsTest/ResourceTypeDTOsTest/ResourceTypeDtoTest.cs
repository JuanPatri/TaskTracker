using Backend.DTOs.ResourceTypeDTOs;

namespace BackendTest.DTOsTest.ResourceTypeDTOsTest;

[TestClass]
public class ResourceTypeDtoTest
{
    [TestMethod]
    public void CreateNameForResourceType()
    {
        ResourceTypeDto resourceTypeDto = new ResourceTypeDto();
        resourceTypeDto.Name = "resourceType";
        Assert.AreEqual("resourceType", resourceTypeDto.Name);
    }
    
    [TestMethod]
    public void CreateIdForResourceType()
    {
        ResourceTypeDto resourceTypeDto = new ResourceTypeDto();
        resourceTypeDto.Id = 1;
        Assert.AreEqual(1, resourceTypeDto.Id);
    }
}