using System.Text.Json;
using DTOs.ExporterDTOs;

namespace Service.ExportService
{
    public class ProjectJsonExporter : IExport<ProjectExporterDataDto>
    {
        public string Export(IEnumerable<ProjectExporterDataDto> projects)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true 
            };

            var orderedProjects = projects.OrderBy(p => p.StartDate)
                .Select(p => new ProjectExporterDataDto
                {
                    Name = p.Name,
                    StartDate = p.StartDate,
                    Tasks = p.Tasks.OrderByDescending(t => t.Title).ToList()
                });

            return JsonSerializer.Serialize(orderedProjects, options);
        }
    }
}