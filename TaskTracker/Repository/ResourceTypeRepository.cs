using Domain;

namespace Repository;

public class ResourceTypeRepository : IRepository<ResourceType>
{
    private readonly SqlContext _sqlContext;

    public ResourceTypeRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;
        
        if (!_sqlContext.ResourceTypes.Any())
        {
            var defaultResourceTypes = new List<ResourceType>()
            {
                new ResourceType()
                {
                    Name = "Human" 
                },
                new ResourceType()
                {
                    Name = "Infrastructure"
                },
                new ResourceType()
                {
                    Name = "Software"
                }
            };

            _sqlContext.ResourceTypes.AddRange(defaultResourceTypes);
            _sqlContext.SaveChanges();
        }
    }

    public ResourceType Add(ResourceType resourceType)
    {
        _sqlContext.ResourceTypes.Add(resourceType);
        _sqlContext.SaveChanges();
        return resourceType;
    }

    public ResourceType? Find(Func<ResourceType, bool> predicate)
    {
        return _sqlContext.ResourceTypes.FirstOrDefault(predicate);
    }

    public IList<ResourceType> FindAll()
    {
        return _sqlContext.ResourceTypes.ToList(); 
    }

    public ResourceType? Update(ResourceType resourceType)
    {
        ResourceType? existingResourceType = _sqlContext.ResourceTypes.FirstOrDefault(r => r.Id == resourceType.Id);
        if (existingResourceType != null)
        {
            existingResourceType.Name = resourceType.Name;
            _sqlContext.SaveChanges(); 
            return existingResourceType;
        }
        return null;
    }

    public void Delete(string entity)
    {
        ResourceType? resourceType = _sqlContext.ResourceTypes.FirstOrDefault(r => r.Id == int.Parse(entity));
        if (resourceType != null)
        {
            _sqlContext.ResourceTypes.Remove(resourceType);
            _sqlContext.SaveChanges(); 
        }
    }
}