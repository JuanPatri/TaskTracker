namespace Backend.Domain;

public class Task
{
    private string _title { get; set; }

    public string ValidateTitle
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("The title name cannot be empty");
            _title = value;
        }
    }
    
}