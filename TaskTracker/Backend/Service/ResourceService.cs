using Backend.Domain;
using Backend.DTOs.ResourceDTOs;
using Backend.Repository;

namespace Backend.Service;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;
    
    public ResourceService(IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository)
    {
        _resourceTypeRepository = resourceTypeRepository;
        _resourceRepository = resourceRepository;
    }
    
    public Resource? AddResource(ResourceDataDto resource)
    {
        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resource.TypeResource);
        Resource? createdResource = _resourceRepository.Add(resource.ToEntity(resourceType));
        return createdResource;
    }
    
    public void RemoveResource(GetResourceDto resource)
    {
        _resourceRepository.Delete(resource.Name);
    }
    
    public Resource? GetResource(GetResourceDto resource)
    {
        return _resourceRepository.Find(r => r.Name == resource.Name);
    }
}