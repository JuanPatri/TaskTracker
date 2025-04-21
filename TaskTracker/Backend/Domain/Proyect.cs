namespace Backend.Domain;

public class Proyect
{
    private string _name { get; set; }

    public Proyect(string name)
    {
        _name = name;
    }
    
    public string getName() => _name;
}