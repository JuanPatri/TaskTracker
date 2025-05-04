using Backend.Domain;

namespace Backend.Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly List<ResourceType> _resourceTypes;
    public ResourceTypeRepository()
    {
        _resourceTypes = new List<ResourceType>();
    }

    public ResourceType Add(ResourceType resourceType)
    {
        _resourceTypes.Add(resourceType);
        return resourceType;
    }

    public ResourceType? Find(Func<ResourceType, bool> predicate)
    {
        return _resourceTypes.FirstOrDefault(predicate);
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