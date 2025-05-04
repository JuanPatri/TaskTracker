using Backend.Domain;

namespace Backend.Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly List<ResourceType> _resourceTypes;
    private int _idCounter;
    public ResourceTypeRepository()
    {
        _idCounter = 1;
        _resourceTypes = new List<ResourceType>()
        {
            new ResourceType()
            {
                Id = _idCounter++,
                Name = "Human"

            },
            new ResourceType()
            {
                Id = _idCounter++,
                Name = "Infrastructure"

            },
            new ResourceType()
            {
                Id = _idCounter++,
                Name = "Software"

            }
        };

    }

    public ResourceType Add(ResourceType resourceType)
    {
        resourceType.Id = _idCounter++;
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

    public ResourceType? Update(ResourceType resourceType)
    {
        ResourceType? existingResourceType = _resourceTypes.FirstOrDefault(r => r.Id == resourceType.Id);
        if (existingResourceType != null)
        {
            existingResourceType.Name = resourceType.Name;
            return existingResourceType;
        }
        return null;
    }

    public void Delete(string entity)
    {
        throw new NotImplementedException();
    }
}