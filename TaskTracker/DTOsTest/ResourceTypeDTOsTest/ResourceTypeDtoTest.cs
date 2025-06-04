using Domain;
using DTOs.ResourceTypeDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTOsTest.ResourceTypeDTOsTest;

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
    
    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    {
        ResourceTypeDto resourceTypeDto = new ResourceTypeDto
        {
            Id = 1,
            Name = "ResourceTypeName"
        };
        
        ResourceType resourceType = ResourceType.Fromdto(resourceTypeDto);
        
        Assert.AreEqual(resourceTypeDto.Id, resourceType.Id);
        Assert.AreEqual(resourceTypeDto.Name, resourceType.Name);
    }
}