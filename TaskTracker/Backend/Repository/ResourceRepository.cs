using Backend.Domain;

namespace Backend.Repository;

public class ResourceRepository : IRepository<Resource>
{
    private readonly List<Resource> _resources;
    
    public ResourceRepository()
    {
        _resources = new List<Resource>();
    }
    public Resource Add(Resource resource)
    {
        _resources.Add(resource);
        return resource;
    }

    public Resource? Find(Func<Resource, bool> predicate)
    {
        return _resources.FirstOrDefault(predicate);
    }

    public IList<Resource> FindAll()
    {
        throw new NotImplementedException();
    }

    public Resource? Update(Resource entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}