using Task = Domain.Task;
using Enums;
using Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository;
using RepositoryTest.Context;

namespace BackendTest.RepositoryTest;

[TestClass]
public class TaskRepositoryTest
{
    private IRepository<Task> _taskRepository;
    private Task _task;
    private SqlContext _sqlContext;

    [TestInitialize]
    public void OnInitialize()
    {
        _sqlContext = SqlContextFactory.CreateMemoryContext();
        _taskRepository = new TaskRepository(_sqlContext);
        _task = new Task(); 
        _task.Title = "Task 1";
    }

    [TestMethod]
    public void AddTaskToListTest()
    {
        _taskRepository.Add(_task);
        
        Task foundTask = _taskRepository.Find(t => t.Title == "Task 1");
        Assert.IsNotNull(foundTask);
        Assert.AreEqual(foundTask, _task);
    }

    [TestMethod]
    public void FindTaskInListTest()
    {
        Task task2 = new Task(); 
        task2.Title = "Task 2";
        
        _taskRepository.Add(_task);
        Task taskFind = _taskRepository.Find(t => t.Title == _task.Title);
        Assert.AreEqual(taskFind, _task);
    }
    
    [TestMethod]
    public void FindAllTasksInListTest()
    {
        _taskRepository.Add(_task);
        
        Assert.AreEqual(1, _taskRepository.FindAll().Count());

        Task task2 = new Task(); 
        task2.Title = "Task 2";
        _taskRepository.Add(task2);

        Assert.AreEqual(2, _taskRepository.FindAll().Count());
    }

[TestMethod]
public void UpdateExistingTaskTest()
{
    _taskRepository.Add(_task);
    
    Task updatedTask = new Task
    {
        Title = "Task 1",
        Description = "Updated Description",
        Duration = 1,  
        Status = Status.Completed,  
    };
    
    Task? originalTask = _taskRepository.Find(t => t.Title == _task.Title);
    Assert.IsNotNull(originalTask);
    Assert.AreEqual(_task.Title, originalTask.Title);
    
    _taskRepository.Update(updatedTask);
    
    Task? foundUpdatedTask = _taskRepository.Find(t => t.Title == "Task 1");
    Assert.IsNotNull(foundUpdatedTask);
    Assert.AreEqual("Task 1", foundUpdatedTask.Title);
    Assert.AreEqual("Updated Description", foundUpdatedTask.Description);
    Assert.AreEqual(Status.Completed, foundUpdatedTask.Status);
}

[TestMethod]
public void DeleteTaskFromListTest(){
    _taskRepository.Add(_task);
    
    Task task2 = new Task(); 
    task2.Title = "Task 2";
    _taskRepository.Add(task2);
    
    Assert.AreEqual(2, _taskRepository.FindAll().Count());
    
    _taskRepository.Delete("Task 1");
    
    Assert.AreEqual(1, _taskRepository.FindAll().Count());
    
}
}