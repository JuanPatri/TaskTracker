using Backend.Domain;
using Backend.Repository;

namespace Backend.Service;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    
    public ResourceService(IRepository<Resource> resourceRepository)
    {
        _resourceRepository = resourceRepository;
    }
    
    
}