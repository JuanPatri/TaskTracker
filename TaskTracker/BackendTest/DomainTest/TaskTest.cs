namespace BackendTest.DomainTest;
using Backend.Domain;
using Backend.Domain.Enums;

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
    public void SetValidTitleUpdatesValueCorrectly()
    {
        string expectedTitle = "Valid Title";
        _task.Title = expectedTitle;
        Assert.AreEqual(expectedTitle, _task.Title);
    }
    
    [TestMethod]
    public void PutTitleNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = null);
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void PutTitleWhitespaceReturnAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Title = " ");
        Assert.AreEqual("The title name cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateDescrptionForTaskTest()
    {
        _task.Description = "Description";
        Assert.AreEqual("Description", _task.Description);
    }

    [TestMethod]
    public void CreateDateForTask()
    {
        DateOnly testDate = new DateOnly(2025, 4, 22);
        _task.Date = testDate;

        Assert.AreEqual(testDate, _task.Date);
    }

    [TestMethod]
    public void SetDateWithFutureDateThrowsArgumentException()
    {
        DateOnly futureDate = new DateOnly(2026, 4, 22);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.Date = futureDate);
        Assert.AreEqual("The date cannot be in the future.", ex.Message);
    }

    [TestMethod]
    public void SetDurationForTask()
    {
        TimeSpan durationTask = new TimeSpan(1, 5, 30, 0);
        
        _task.DurationTask = durationTask;

        Assert.AreEqual(durationTask, _task.DurationTask);
    }

    [TestMethod]
    public void SetStatusForTask()
    {
        _task.Status = Status.Pending;
        Assert.AreEqual(Status.Pending, _task.Status);
    }


    [TestMethod]
    public void SetEarliestStartDateForTask()
    {
        DateTime earlistStartDate = new DateTime(2026, 4, 22, 5, 30, 0);
        _task.EarliestStartDate = earlistStartDate;
        Assert.AreEqual(earlistStartDate, _task.EarliestStartDate);
    }

    [TestMethod]
    public void SetEarliestStartDateThrowsArgumentExceptionForPastDate()
    {
        DateTime pastDate = new DateTime(2024, 4, 22, 5, 30, 0);

        ArgumentException ex =
            Assert.ThrowsException<ArgumentException>(() => _task.EarliestStartDate = pastDate);
        Assert.AreEqual("The date cannot be in the past.", ex.Message);
    }

    [TestMethod]
    public void SetEarliestEndDateForTask()
    {
        DateTime earlistEndDate = new DateTime(2026, 4, 22, 5, 30, 0);
        _task.EarliestEndDate = earlistEndDate;

        Assert.AreEqual(earlistEndDate, _task.EarliestEndDate);
    }

    [TestMethod]
    public void SetEarliestEndDateThrowsArgumentExceptionForPastDate()
    {
        DateTime pastDate = new DateTime(2024, 4, 22, 5, 30, 0);

        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _task.EarliestEndDate = pastDate);
        Assert.AreEqual("The date cannot be in the past.", ex.Message);
    }
    
}