using Backend.Domain;
namespace BackendTest.DomainTest;

[TestClass]
public class ProyectTest
{
    private Proyect _proyect;
    [TestInitialize]
    public void OnInitialize()
    {
        _proyect = new Proyect();
    }
    
    [TestMethod]
    public void CreateProyect()
    {
        Assert.IsNotNull(_proyect);
    }
    
    [TestMethod]
    public void CreateNameForProyect()
    {
        _proyect.ValidateName = "Proyect1";
        Assert.AreEqual("Proyect1", _proyect.ValidateName);
    }
    
    [TestMethod]
    public void IPutNameNullReturnsAnException()
    {
        ArgumentException ex = Assert.ThrowsException<ArgumentException>(() => _proyect.ValidateName = null);
        Assert.AreEqual("The proyectname cannot be empty", _proyect.ValidateName);
    }
}