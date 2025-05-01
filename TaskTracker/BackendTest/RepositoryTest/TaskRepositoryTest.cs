namespace BackendTest.RepositoryTest;
using Backend.Domain;
using Backend.Repository;

[TestClass]
public class TaskRepositoryTest
{
    
    private TaskRepository _taskRepository;
    private Task _task;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _task = new Task();
        _task.Title = "Task 1";
    }
    
    [TestMethod]
    public void CreateTaskRepository()
    { 
        Assert.IsNotNull(_taskRepository);
    }

}