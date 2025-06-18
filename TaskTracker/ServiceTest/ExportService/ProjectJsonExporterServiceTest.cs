using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.ExportService;
using DTOs.ExporterDTOs;
using System.Text.Json;

namespace ServiceTest.ExportService
{
    [TestClass]
    public class ProjectJsonExporterServiceTest
    {
        private ProjectJsonExporter _exporter;

        [TestInitialize]
        public void Setup()
        {
            _exporter = new ProjectJsonExporter();
        }

        [TestMethod]
        public void Export_WithMultipleProjectsAndTasks_ReturnsOrderedJson()
        {
            var projects = new List<ProjectExporterDataDto>
            {
                new ProjectExporterDataDto
                {
                    Name = "Project B",
                    StartDate = new DateOnly(2025, 4, 10),
                    Tasks = new List<TaskExporterDataDto>
                    {
                        new TaskExporterDataDto { Title = "Beta", StartDate = DateTime.Today, IsCritical = "N" },
                        new TaskExporterDataDto { Title = "Alpha", StartDate = DateTime.Today, IsCritical = "Y" }
                    }
                },
                new ProjectExporterDataDto
                {
                    Name = "Project A",
                    StartDate = new DateOnly(2025, 3, 5),
                    Tasks = new List<TaskExporterDataDto>
                    {
                        new TaskExporterDataDto { Title = "Gamma", StartDate = DateTime.Today, IsCritical = "Y" }
                    }
                }
            };

            string jsonResult = _exporter.Export(projects);

            var deserialized = JsonSerializer.Deserialize<List<ProjectExporterDataDto>>(jsonResult);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(2, deserialized.Count);
            Assert.AreEqual("Project A", deserialized[0].Name); 
            Assert.AreEqual("Gamma", deserialized[0].Tasks.First().Title);

            var taskTitles = deserialized[1].Tasks.Select(t => t.Title).ToList();
            CollectionAssert.AreEqual(new List<string> { "Beta", "Alpha" }.OrderByDescending(x => x).ToList(), taskTitles);
        }

        [TestMethod]
        public void Export_WithNoProjects_ReturnsEmptyJsonArray()
        {
            var jsonResult = _exporter.Export(new List<ProjectExporterDataDto>());

            Assert.AreEqual("[]", jsonResult.Trim());
        }
    }
}
