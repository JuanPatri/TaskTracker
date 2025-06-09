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
            var csvBuilder = new StringBuilder();

            var orderedProjects = projects.OrderBy(p => p.StartDate);

            foreach (var project in orderedProjects)
            {
                csvBuilder.AppendLine($"{project.Name},{project.StartDate:dd/MM/yyyy}");

                var orderedTasks = project.Tasks.OrderByDescending(t => t.Title);

                foreach (var task in orderedTasks)
                {
                    csvBuilder.AppendLine($"{task.Title},{task.StartDate:dd/MM/yyyy},{task.IsCritical}");

                    foreach (var resource in task.Resources)
                    {
                        csvBuilder.AppendLine(resource);
                    }
                }
            }

            return csvBuilder.ToString();
        }
    }
}