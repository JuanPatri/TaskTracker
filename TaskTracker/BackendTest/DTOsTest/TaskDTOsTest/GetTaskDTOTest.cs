using Backend.DTOs.TaskDTOs;

namespace BackendTest.DTOsTest.TaskDTOsTest;

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