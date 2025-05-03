using Backend.DTOs.TaskDTOs;
using Task = Backend.Domain.Task;

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

    [TestMethod]
    public void ToEntityShouldReturnTitle()
    {
        GetTaskDTO taskDto = new GetTaskDTO();
        taskDto.Title = "Test";
        Task task = taskDto.ToEntity();
        Assert.AreEqual("Test", task.Title);
    }
}