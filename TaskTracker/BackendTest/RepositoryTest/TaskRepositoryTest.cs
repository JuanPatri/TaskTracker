using Backend.Domain;

namespace BackendTest.RepositoryTest;

using Backend.Repository;

[TestClass]
public class TaskRepositoryTest
{
    private TaskRepository _taskRepository;
    private ProjectTask _projectTask;

    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _projectTask = new ProjectTask();
        _projectTask.Title = "Task 1";
    }

    [TestMethod]
    public void AddTaskToListTest()
    {
        _taskRepository.Add(_projectTask);
    
        ProjectTask foundProjectTask = _taskRepository.Find(t => t.Title == "Task 1");
        Assert.IsNotNull(foundProjectTask);
        Assert.AreEqual(_projectTask, foundProjectTask);
    }

}