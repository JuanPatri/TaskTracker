namespace BackendTest.DTOsTest.TaskDTOsTest;
using Backend.DTOs.TaskDTOs;

public class TaskDataDtoTest
{
    [TestMethod]
    public void SetTitleForTask()
    {
        TaskDataDTO taskDto = new TaskDataDTO();
        taskDto.Title = "Task 1";
        Assert.AreEqual("Task 1", taskDto.Title);
    }
}