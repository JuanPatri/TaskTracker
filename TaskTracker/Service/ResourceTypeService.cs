using Domain;
using DTOs.ResourceTypeDTOs;
using Repository;

namespace Service;

public class ResourceTypeService
{
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    private int _idResourceType;

    public ResourceTypeService(IRepository<ResourceType> resourceTypeRepository)
    {
        _idResourceType = 4;
        _resourceTypeRepository = resourceTypeRepository;
    }

    
    public ResourceType? AddResourceType(ResourceTypeDto resourceType)
    {
        if (_resourceTypeRepository.Find(r => r.Name == resourceType.Name) != null)
        {
            throw new Exception("Resource type already exists");
        }

        resourceType.Id = _idResourceType++;
        ResourceType? createdResourceType = _resourceTypeRepository.Add(ResourceType.Fromdto(resourceType));
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
        ResourceType? updatedResourceType = _resourceTypeRepository.Update(ResourceType.Fromdto(resourceTypeDto));
        return updatedResourceType;
    }
    

}