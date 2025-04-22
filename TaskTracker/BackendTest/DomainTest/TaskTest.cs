namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class TaskTest
{
    private Task _task;

    [TestInitialize]
    public void OnInitialize()
    {
        _task = new Task();
    }
    
    [TestMethod]
    public void CreateTaskTest()
    {
        Assert.IsNotNull(_task);
    }

    [TestMethod]
    public void CreateTitleForTask()
    {
        _task.ValidateTitle = "Title2";
        Assert.AreEqual("Title2", _task.ValidateTitle);
    }

    [TestMethod]
    public void IPutTitleNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.ValidateTitle = null);
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void IPutTitleWhitespaceReturnAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.ValidateTitle = " ");
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void IPutDateForTask()
    {
        var expectedDate = DateTime.Now;
        _task.Date = DateTime.Now;
        
        Assert.IsTrue(Math.Abs((expectedDate - _task.Date).TotalSeconds) < 1);
    }
}