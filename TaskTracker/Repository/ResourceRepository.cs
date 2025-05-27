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
        return _resources;
    }

    public Resource? Update(Resource entity)
    {
        Resource? existingResource = _resources.FirstOrDefault(r => r.Name == entity.Name);
        if (existingResource != null)
        {
            existingResource.Name = entity.Name;
            existingResource.Description = entity.Description;
            existingResource.Type = entity.Type;
            return existingResource;
        }
        return null;
    }

    public void Delete(string name)
    {
        Resource? resource = _resources.FirstOrDefault(r => r.Name == name);
        if (resource != null)
        {
            _resources.Remove(resource);
        }
    }
}