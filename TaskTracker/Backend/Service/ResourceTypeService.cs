using Backend.Domain;
using Backend.DTOs.ResourceTypeDTOs;
using Backend.Repository;

namespace Backend.Service;

public class ResourceTypeService
{
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private int _id;
    
    public ResourceTypeService(IRepository<ResourceType> resourceTypeRepository)
    {
        _resourceTypeRepository = resourceTypeRepository;
        _id = 4;
    }
    
    public ResourceType? AddResourceType(ResourceTypeDto resourceType)
    {
        resourceType.Id = _id++;
        ResourceType? createdResourceType = _resourceTypeRepository.Add(resourceType.ToEntity());
        return createdResourceType;
    }
    
    public void RemoveResourceType(ResourceTypeDto resourceType)
    {
        _resourceTypeRepository.Delete(resourceType.Id.ToString());
    }
    
    public ResourceType? GetResourceType(ResourceTypeDto resourceType)
    {
        return _resourceTypeRepository.Find(r => r.Id == resourceType.Id);
    }
    
    public List<ResourceType> GetAllResourcesType()
    {
        return _resourceTypeRepository.FindAll().ToList();
    }
    
    public ResourceType? UpdateResourceType(ResourceTypeDto resourceTypeDto)
    {
        ResourceType? updatedResourceType = _resourceTypeRepository.Update(resourceTypeDto.ToEntity());
        return updatedResourceType;
    }
}