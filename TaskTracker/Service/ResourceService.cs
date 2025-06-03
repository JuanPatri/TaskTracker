using Domain;
using DTOs.ResourceDTOs;
using Enums;
using Repository;
using Task = System.Threading.Tasks.Task;

namespace Service;

public class ResourceService
{
    private readonly IRepository<Resource> _resourceRepository;
    private readonly IRepository<ResourceType> _resourceTypeRepository;

    
    public ResourceService(IRepository<Resource> resourceRepository, IRepository<ResourceType> resourceTypeRepository)
    {
        _resourceRepository = resourceRepository;
        _resourceTypeRepository = resourceTypeRepository;
    }
    
    public Resource? AddResource(ResourceDataDto resource)
    {
        if (_resourceRepository.Find(r => r.Name == resource.Name) != null)
        {
            throw new Exception("Resource already exists");
        }

        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resource.TypeResource);
        Resource? createdResource = _resourceRepository.Add(Resource.FromDto(resource, resourceType));
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

    public List<Resource> GetAllResources()
    {
        return _resourceRepository.FindAll().ToList();
    }

    public Resource? UpdateResource(ResourceDataDto resourceDto)
    {
        ResourceType? resourceType = _resourceTypeRepository.Find(r => r.Id == resourceDto.TypeResource);
        Resource? updatedResource = _resourceRepository.Update(Resource.FromDto(resourceDto, resourceType));
        return updatedResource;
    }

    public List<GetResourceDto> GetResourcesForSystem()
    {
        return _resourceRepository.FindAll()
            .Select(resource => new GetResourceDto { Name = resource.Name })
            .ToList();
    }
    
    

}