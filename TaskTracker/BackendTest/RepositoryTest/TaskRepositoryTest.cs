using Backend.Repository;
using TaskItem = Backend.Domain.Task; 
using Backend.Domain;
using Backend.Domain.Enums;
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
    
    [TestMethod]
    public void FindAllTasksInListTest()
    {
        _taskRepository.Add(_task);
        
        Assert.AreEqual(1, _taskRepository.FindAll().Count());

        TaskItem task2 = new TaskItem(); 
        task2.Title = "Task 2";
        _taskRepository.Add(task2);

        Assert.AreEqual(2, _taskRepository.FindAll().Count());
    }

[TestMethod]
public void UpdateExistingTaskTest()
{
    _taskRepository.Add(_task);
    
    TaskItem updatedTask = new TaskItem
    {
        Title = "Task 1",
        Description = "Updated Description",
        Duration = 0.5,  
        Status = Status.Completed,  
        Dependencies = new List<TaskItem>()  
    };
    
    TaskItem? originalTask = _taskRepository.Find(t => t.Title == _task.Title);
    Assert.IsNotNull(originalTask);
    Assert.AreEqual(_task.Title, originalTask.Title);
    
    _taskRepository.Update(updatedTask);
    
    TaskItem? foundUpdatedTask = _taskRepository.Find(t => t.Title == "Task 1");
    Assert.IsNotNull(foundUpdatedTask);
    Assert.AreEqual("Task 1", foundUpdatedTask.Title);
    Assert.AreEqual("Updated Description", foundUpdatedTask.Description);
    Assert.AreEqual(Status.Completed, foundUpdatedTask.Status);
}

[TestMethod]
public void DeleteTaskFromListTest(){
    _taskRepository.Add(_task);
    
    TaskItem task2 = new TaskItem(); 
    task2.Title = "Task 2";
    _taskRepository.Add(task2);
    
    Assert.AreEqual(2, _taskRepository.FindAll().Count());
    
    _taskRepository.Delete("Task 1");
    
    Assert.AreEqual(1, _taskRepository.FindAll().Count());
    
}
}