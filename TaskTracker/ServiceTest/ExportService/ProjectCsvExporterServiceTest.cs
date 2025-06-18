using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.ExportService;
using DTOs.ExporterDTOs;

namespace ServiceTest.ExportService
{
    [TestClass]
    public class ProjectCsvExporterServiceTest
    {
        private ProjectCsvExporter _exporter;

        [TestInitialize]
        public void Inizializate()
        {
            _exporter = new ProjectCsvExporter();
        }

        [TestMethod]
        public void Export_WithExampleProjectAndTasks_ReturnsCorrectCsv()
        {
            var project = new ProjectExporterDataDto
            {
                Name = "Example Project",
                StartDate = new DateOnly(2025, 3, 1),
                Tasks = new List<TaskExporterDataDto>
                {
                    new TaskExporterDataDto
                    {
                        Title = "Task Alpha",
                        StartDate = new DateTime(2025, 3, 5),
                        IsCritical = "Y",
                        Resources = new List<string> { "John", "Sarah" }
                    },
                    new TaskExporterDataDto
                    {
                        Title = "Task Beta",
                        StartDate = new DateTime(2025, 3, 2),
                        IsCritical = "N",
                        Resources = new List<string> { "Emma" }
                    }
                }
            };

            var expectedCsv = 
@"Example Project,01/03/2025
Task Beta,02/03/2025,N
Emma
Task Alpha,05/03/2025,Y
John
Sarah
";

            var result = _exporter.Export(new List<ProjectExporterDataDto> { project });

            Assert.AreEqual(expectedCsv, result);
        }

        [TestMethod]
        public void Export_WithNoProjects_ReturnsEmptyString()
        {
            var result = _exporter.Export(new List<ProjectExporterDataDto>());

            Assert.AreEqual(string.Empty, result);
        }
    }
}
