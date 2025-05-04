using Backend.Domain;

namespace Backend.Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly List<ResourceType> _resourceTypes;
    public ResourceTypeRepository()
    {
        _resourceTypes = new List<ResourceType>();
    }

    public ResourceType Add(ResourceType entity)
    {
        throw new NotImplementedException();
    }

    public ResourceType? Find(Func<ResourceType, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public IList<ResourceType> FindAll()
    {
        throw new NotImplementedException();
    }

    public ResourceType? Update(ResourceType entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}