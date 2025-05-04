using Backend.Domain;

namespace Backend.Repository;

public class ResourceRepository : IRepository<Resource>
{
    public Resource Add(Resource entity)
    {
        throw new NotImplementedException();
    }

    public Resource? Find(Func<Resource, bool> predicate)
    {
        throw new NotImplementedException();
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