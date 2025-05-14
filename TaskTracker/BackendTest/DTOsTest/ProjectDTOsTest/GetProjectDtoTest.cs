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
    
}