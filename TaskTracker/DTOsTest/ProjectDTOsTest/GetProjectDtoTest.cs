using DTOs.ProjectDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTOsTest.ProjectDTOsTest;

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