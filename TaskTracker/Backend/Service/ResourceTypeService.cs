using Backend.Domain;
using Backend.Repository;

namespace Backend.Service;

public class ResourceTypeService
{
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    
    public ResourceTypeService(IRepository<ResourceType> resourceTypeRepository)
    {
        _resourceTypeRepository = resourceTypeRepository;
    }
}