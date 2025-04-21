namespace Backend.Domain;

public class User
{
    private string _name { get; set; }

    public User(string name)
    {
        _name = name;
    }
    public string getName() => _name;
}