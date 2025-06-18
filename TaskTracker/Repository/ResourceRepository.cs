using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ResourceRepository : IRepository<Resource>
{
    private readonly SqlContext _sqlContext;
    
    public ResourceRepository(SqlContext sqlContext)
    {
        _sqlContext = sqlContext;
    }
    public Resource Add(Resource resource)
    {
        _sqlContext.Resources.Add(resource);
        _sqlContext.SaveChanges();
        return resource;
    }

    public Resource? Find(Func<Resource, bool> predicate)
    {
        return _sqlContext.Resources
            .Include(r => r.Type) 
            .FirstOrDefault(predicate);
    }

    public IList<Resource> FindAll()
    {
        return _sqlContext.Resources
            .Include(r => r.Type)
            .ToList();
    }

    public Resource? Update(Resource entity)
    {
        Resource? existingResource = _sqlContext.Resources.FirstOrDefault(r => r.Id == entity.Id);
        if (existingResource != null)
        {
            existingResource.Name = entity.Name;
            existingResource.Description = entity.Description;
            existingResource.Type = entity.Type;
            existingResource.Quantity = entity.Quantity; 
        
            _sqlContext.SaveChanges(); 
            return existingResource;
        }
        return null;
    }

    public void Delete(string name)
    {
        Resource? resource = _sqlContext.Resources.FirstOrDefault(r => r.Name == name);
        if (resource != null)
        {
            _sqlContext.Resources.Remove(resource);
            _sqlContext.SaveChanges();
        }
    }
}