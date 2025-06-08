using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTOs.ExporterDTOs;

namespace Service.ExportService
{
    public class ProjectCsvExporter : IExport<ProjectExporterDataDto>
    {
        public string Export(IEnumerable<ProjectExporterDataDto> projects)
        {
           
        }
    }
}