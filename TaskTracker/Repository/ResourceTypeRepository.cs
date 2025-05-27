using Backend.Domain;

namespace Backend.Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly List<ResourceType> _resourceTypes;
    //private int _idCounter;
    public ResourceTypeRepository()
    {
        //_idCounter = 1;
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
        //resourceType.Id = _idCounter++;
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
        ResourceType? resourceType = _resourceTypes.FirstOrDefault(r => r.Id == int.Parse(entity));
        if (resourceType != null)
        {
            _resourceTypes.Remove(resourceType);
        }
    }
}