using System.Text.Json;
using DTOs.ExporterDTOs;

namespace Service.ExportService
{
    public class ProjectJsonExporter : IExport<ProjectExporterDataDto>
    {
        public string Export(IEnumerable<ProjectExporterDataDto> projects)
        {
            
        }
    }
}