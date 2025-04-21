namespace Backend.Domain;

public class User
{
    private string _name { get; set; }

    public string ValidateName
    {
        get => _name;
        set => _name = value;
    }
}