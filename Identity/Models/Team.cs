namespace Identity.Models;

public class Team
{
    private static Random random = new();

    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public Employee Manager { get; set; }

    public List<Employee> Members { get; set; } = [];


    public Team()
    {
        Id = GenerateRandomId();
    }

    private int GenerateRandomId()
    {
        // Generates a 6-digit number between 100000 and 999999
        return random.Next(100000, 1000000); 
    }
}
