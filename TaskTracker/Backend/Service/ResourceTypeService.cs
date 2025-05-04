using Backend.Domain;
using Backend.DTOs.ResourceTypeDTOs;
using Backend.Repository;

namespace Backend.Service;

public class ResourceTypeService
{
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    
    public ResourceTypeService(IRepository<ResourceType> resourceTypeRepository)
    {
        _resourceTypeRepository = resourceTypeRepository;
    }
    
    public ResourceType? AddResourceType(ResourceTypeDto resourceType)
    {
        ResourceType? createdResourceType = _resourceTypeRepository.Add(resourceType.ToEntity());
        return createdResourceType;
    }
    
    public void RemoveResourceType(ResourceTypeDto resourceType)
    {
        _resourceTypeRepository.Delete(resourceType.Id.ToString());
    }
}