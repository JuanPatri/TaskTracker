using DTOs.TaskDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTOsTest.TaskDTOsTest;

[TestClass]
public class GetTaskDTOTest
{
    [TestMethod]
    public void AddTitleToTask()
    {
        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test";
        Assert.AreEqual("Test", taskDto.Title);
    }
}