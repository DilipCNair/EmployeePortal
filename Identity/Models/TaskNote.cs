namespace Identity.Models;

public class TaskNote
{
    private static Random random = new();

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime Created { get; set; }

    public string Note { get; set; }

    public TaskNote()
    {
        Id = random.Next(100000, 1000000);
        Created = DateTime.UtcNow;
    }
}
