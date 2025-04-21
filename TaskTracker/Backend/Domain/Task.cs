namespace Backend.Domain;

public class Task
{
    private string _title { get; set; }

    public Task(string title)
    {
        _title = title;
    }

    public string GetTitle() => _title;
}