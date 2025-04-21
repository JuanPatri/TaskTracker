namespace Backend.Domain;

public class Task
{
    private string _title { get; set; }

    public string ValidateTitle
    {
        get => _title;
        set => _title = value;
    }
    
}