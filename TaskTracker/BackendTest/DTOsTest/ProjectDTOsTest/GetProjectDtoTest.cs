using Backend.DTOs.ProjectDTOs;

namespace BackendTest.DTOsTest.ProjectDTOsTest;

[TestClass]
public class GetProjectDtoTest
{
    [TestMethod]
    public void AddIdToProjectTest()
    {
        GetProjectDTO projectDto = new GetProjectDTO();
        projectDto.Id = 1;
        Assert.AreEqual(1, projectDto.Id);
    }
    
    [TestMethod]
    public void ToEntityShouldReturnProjectNameTest()
    {
        var dto = new GetProjectDTO()
        {
            Id = 1,
            Name = "Project A"
        };
        var result = dto.ToEntity();
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Project A", result.Name);
    }
    
}