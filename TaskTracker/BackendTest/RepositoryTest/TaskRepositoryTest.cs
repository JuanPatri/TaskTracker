using Backend.Repository;
using TaskItem = Backend.Domain.Task; 

namespace BackendTest.RepositoryTest;

[TestClass]
public class TaskRepositoryTest
{
    private IRepository<TaskItem> _taskRepository;
    private TaskItem _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _taskRepository = new TaskRepository();
        _task = new TaskItem(); 
        _task.Title = "Task 1";
    }

    [TestMethod]
    public void AddTaskToListTest()
    {
        _taskRepository.Add(_task);
        
        TaskItem foundTask = _taskRepository.Find(t => t.Title == "Task 1");
        Assert.IsNotNull(foundTask);
        Assert.AreEqual(foundTask, _task);
    }

    [TestMethod]
    public void FindTaskInListTest()
    {
        TaskItem task2 = new TaskItem(); 
        task2.Title = "Task 2";
        
        _taskRepository.Add(_task);
        TaskItem taskFind = _taskRepository.Find(t => t.Title == _task.Title);
        Assert.AreEqual(taskFind, _task);
    }
}