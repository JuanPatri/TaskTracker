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
        DateOnly testDate = new DateOnly(2025, 4, 22);
        _task.ValidateDate = testDate;
        
        Assert.AreEqual(testDate, _task.ValidateDate);
    }

    [TestMethod]
    public void ISetDateWithFutureDateThrowsArgumentException()
    {
        DateOnly futureDate = new DateOnly(2026, 4, 22);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.ValidateDate = futureDate);
        Assert.AreEqual("The date cannot be in the future.", ex.Message);
    }
}