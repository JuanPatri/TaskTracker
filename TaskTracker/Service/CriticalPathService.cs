using Domain;
using Enums;
using Repository;
using Task = Domain.Task;

namespace Service;

public class CriticalPathService
{
    private readonly IRepository<Task> _taskRepository;
    private readonly IRepository<Project> _projectRepository;
    
    public CriticalPathService(IRepository<Project> projectRepository, IRepository<Task> taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }
    private bool BuildCriticalPath(Task current, List<Task> allTasks, List<Task> criticalTasks, List<Task> path)
    {
        path.Add(current);

        List<Task> successors = allTasks
            .Where(t => t.Dependencies.Any(d => d.Dependency == current) && criticalTasks.Contains(t))
            .ToList();

        if (successors.Count == 0)
        {
            return true;
        }

        foreach (var next in successors)
        {
            if (BuildCriticalPath(next, allTasks, criticalTasks, path))
                return true;
        }

        path.Remove(current);
        return false;
    }

    
    public List<Task> GetCriticalPath(Project project)
    {
        CalculateLateTimes(project);

        List<Task> criticalTasks = new List<Task>();
        foreach (var t in project.Tasks)
        {
            if (t.EarlyStart == t.LateStart)
            {
                criticalTasks.Add(t);
            }
        }

        List<Task> startTasks = new List<Task>();
        foreach (var t in project.Tasks)
        {
            List<Task> internalDependencies = t.Dependencies?
                .Select(dep => dep.Dependency)
                .Where(dep => project.Tasks.Contains(dep))
                .ToList() ?? new List<Task>();

            if (!internalDependencies.Any() && criticalTasks.Contains(t))
            {
                startTasks.Add(t);
            }
        }

        foreach (var start in startTasks)
        {
            List<Task> path = new List<Task>();
            if (BuildCriticalPath(start, project.Tasks, criticalTasks, path))
            {
                return path;
            }
        }

        return new List<Task>();
    }

    
    private void TopologicalSortDFS(Task task, List<Task> visited, List<Task> tempVisited, List<Task> result)
    {
        if (tempVisited.Contains(task))
        {
            throw new InvalidOperationException("Cycle detected in task dependencies");
        }

        if (visited.Contains(task))
        {
            return;
        }

        tempVisited.Add(task);

        if (task.Dependencies != null)
        {
            foreach (var dependency in task.Dependencies)
            {
                TopologicalSortDFS(dependency.Dependency, visited, tempVisited, result);
            }
        }

        tempVisited.Remove(task);
        visited.Add(task);
        result.Add(task);
    }
    
    private List<Task> GetTopologicalOrder(List<Task> tasks)
    {
        var visited = new List<Task>();
        var tempVisited = new List<Task>();
        var result = new List<Task>();

        foreach (var task in tasks)
        {
            if (!visited.Contains(task))
            {
                TopologicalSortDFS(task, visited, tempVisited, result);
            }
        }

        return result;
    }

    public void CalculateLateTimes(Project project)
    {
        CalculateEarlyTimes(project);

        if (project.Tasks == null || !project.Tasks.Any())
            throw new InvalidOperationException("The project has no defined tasks.");

        DateTime maxEF = project.Tasks.Max(t => t.EarlyFinish);

        foreach (var task in project.Tasks)
        {
            task.LateFinish = maxEF;
            task.LateStart = maxEF.AddDays(-task.Duration);
        }

        List<Task> ordered = GetTopologicalOrder(project.Tasks);
        ordered.Reverse();

        foreach (var task in ordered)
        {
            
            List<Task> successors = project.Tasks
                .Where(t => t.Dependencies != null && t.Dependencies.Any(dep => dep.Dependency == task))
                .ToList();

            if (successors.Any())
            {
                DateTime earliestSuccessorStart = successors.Min(s => s.LateStart);
                task.LateFinish = earliestSuccessorStart;
                task.LateStart = task.LateFinish.AddDays(-task.Duration);
            }
        }
    }
    
    public void CalculateEarlyTimes(Project project)
    {
        var orderedTasks = GetTopologicalOrder(project.Tasks);

        foreach (var task in project.Tasks)
        {
            if (task.Status != Status.Completed)
            {
                task.EarlyStart = default;
                task.EarlyFinish = default;
            }
        }

        foreach (var task in orderedTasks)
        {
            if (task.Status == Status.Completed)
                continue;

            DateTime es;

            if (task.Dependencies == null || task.Dependencies.Count == 0)
            {
                es = project.StartDate.ToDateTime(new TimeOnly(0, 0));
            }
            else
            {
                DateTime latestDependencyFinish = task.Dependencies.Max(dep => dep.Dependency.EarlyFinish);
                es = latestDependencyFinish;
            }

            task.EarlyStart = es;
            task.EarlyFinish = es.AddDays(task.Duration);
        }
    }
}