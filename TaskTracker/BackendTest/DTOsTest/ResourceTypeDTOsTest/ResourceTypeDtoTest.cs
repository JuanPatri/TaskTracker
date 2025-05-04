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

}