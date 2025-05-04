using Backend.Domain;

namespace BackendTest.DTOsTest.TaskDTOsTest;
using Backend.DTOs.TaskDTOs;
using Backend.Domain.Enums;
using Backend.Domain;

[TestClass]
public class TaskDataDtoTest
{

    private TaskDataDTO _taskDataDto;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskDataDto = new TaskDataDTO();
    }
    
    [TestMethod]
    public void SetTitleForTask()
    {
        _taskDataDto.Title = "Task 1";
        Assert.AreEqual("Task 1", _taskDataDto.Title);
    }

    [TestMethod]
    public void SetDescriptionForTask()
    {
        _taskDataDto.Description = "Description of Task 1";
        Assert.AreEqual("Description of Task 1", _taskDataDto.Description);
    }
    
    [TestMethod]
    public void SetDurationForTask()
    {
        _taskDataDto.Duration = TimeSpan.FromHours(1);
        Assert.AreEqual(TimeSpan.FromHours(1), _taskDataDto.Duration);
    }
    
    [TestMethod]
    public void SetStatusForTask()
    {
        _taskDataDto.Status = Status.Blocked;
        Assert.AreEqual(Status.Blocked, _taskDataDto.Status);
    }
    [TestMethod]
    public void SetProjectForTask()
    {
        _taskDataDto.Project = 9;
        Assert.AreEqual(9, _taskDataDto.Project);
    }
    
    [TestMethod]
    public void SetDependenciesForTask()
    {
        List<String> dependencies = new List<String> { "Task 1", "Task 2" };;
        _taskDataDto.Dependencies = dependencies;
        Assert.AreEqual(dependencies, _taskDataDto.Dependencies);
    }
    
    [TestMethod]
    public void ToEntityShouldMapAllPropertiesCorrectly()
    { 
        _taskDataDto = new TaskDataDTO
        {
            Title = "Task",
            Description = "Description task",
            Duration = TimeSpan.FromHours(1),
            Status = Status.Blocked,
            Project = 9,
            Dependencies = new List<String> { "Task 1", "Task 2" }
        };

        List<Task> dependencies = new List<Task>()
        {
            new Task()
            {
                Title = _taskDataDto.Dependencies[0]
            },
            new Task()
            {
                Title = _taskDataDto.Dependencies[1]
            }
        };
        Project project = new Project()
        {
            Id = _taskDataDto.Project
        };
        
        Task task = _taskDataDto.ToEntity(dependencies, project);

        Assert.IsNotNull(task);
    }
}