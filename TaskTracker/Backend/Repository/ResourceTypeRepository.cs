using Backend.Domain;

namespace Backend.Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly List<ResourceType> _resourceTypes;
    public ResourceTypeRepository()
    {
        _resourceTypes = new List<ResourceType>()
        {
            new ResourceType()
            {
                Id = 1,
                Name = "Human"

            },
            new ResourceType()
            {
                Id = 2,
                Name = "Infrastructure"

            },
            new ResourceType()
            {
                Id = 3,
                Name = "Software"

            }
        };

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
        return _resourceTypes; 
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