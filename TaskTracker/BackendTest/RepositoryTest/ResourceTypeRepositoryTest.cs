using Backend.Domain;
using Backend.Repository;

namespace BackendTest.RepositoryTest;

[TestClass]
public class ResourceTypeRepositoryTest
{
    private ResourceTypeRepository _resourceTypeRepository;
    private ResourceType _resourceType;
    
    [TestInitialize]
    public void OnInitialize()
    {
        _resourceTypeRepository = new ResourceTypeRepository();
        _resourceType = new ResourceType();
        _resourceType.Name = "new resource type";
    }
    [TestMethod]
    public void CreateResourceRepository()
    { 
        Assert.IsNotNull(_resourceTypeRepository);
    }
    
    [TestMethod]
    public void AddResourceTypeToRepository()
    {
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceTypeRepository.Find(r => r.Name == "new resource type"), _resourceType);
    }
    
    [TestMethod]
    public void SearchForAllResourceTypeInTheList()
    {
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 3);
        _resourceTypeRepository.Add(_resourceType);
        Assert.AreEqual(_resourceTypeRepository.FindAll().Count, 4);
    }

}