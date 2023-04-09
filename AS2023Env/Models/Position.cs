namespace AS2023Env.Models;

public class Position
{
    public string Id { get; }
    public string Name { get; }

    public Position(string id, string name)
    {
        Id = id;
        Name = name;
    }
}