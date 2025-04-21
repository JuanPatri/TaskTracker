namespace BackendTest.DomainTest;
using Backend.Domain;

[TestClass]
public class UserTest
{
    private User _user;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _user = new User();
    }
    
    [TestMethod]
    public void CreateUser()
    {
        Assert.IsNotNull(_user);
    }
    
    [TestMethod]
    public void CreateNameForUser()
    {
        _user.Name = "Pedro";
        Assert.AreEqual("Pedro", _user.Name);
    }
    
    [TestMethod]
    public void PutNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Name = null);
        Assert.AreEqual("The username cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void CreateLastNameForUser()
    {
        _user.LastName = "Rodriguez";
        Assert.AreEqual("Rodriguez", _user.LastName);
    }
    
    [TestMethod]
    public void PutLastNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.LastName = null);
        Assert.AreEqual("The user last name cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void AddEmailToUser()
    {
        _user.Email = "prodriguez@gmail.com";
        Assert.AreEqual("prodriguez@gmail.com", _user.Email);
    }
    
    [TestMethod]
    public void PutEmailNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Email = null);
        Assert.AreEqual("The email cannot be empty", ex.Message);
    }
    
    [TestMethod]
    public void DontPutAnArrobaInEmail()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Email  = "prodriguezgmail.com");
        Assert.AreEqual("Error in the email format", ex.Message);
    }
    
    [TestMethod]
    public void DontPutAnPointInEmail()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Email  = "prodriguez@gmailcom");
        Assert.AreEqual("Error in the email format", ex.Message);
    }
    [TestMethod]
    public void AddBirthDateToUser()
    {
        DateTime fecha = new DateTime(2003, 03, 14);
        _user.BirthDate = fecha;
        Assert.AreEqual(fecha, _user.BirthDate);
    }
    
    [TestMethod]
    public void PutBirthDateNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.BirthDate = default);
        Assert.AreEqual("The birth date cannot be empty", ex.Message);
    }

    [TestMethod]
    public void addPasswordToUser()
    {
        _user.Password = "Pedro1234@";
        Assert.AreEqual("Pedro1234@", _user.Password);
    }
    
    [TestMethod]
    public void PutPasswordNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _user.Password = null);
        Assert.AreEqual("The password format is not correct", ex.Message);
    }
}