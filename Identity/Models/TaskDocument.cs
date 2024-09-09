namespace Identity.Models;

public class TaskDocument
{
    public int Id { get; set; }

    public string? FileURI { get; set; }

    public string? FileName { get; set; }

    public string? FileExtension { get; set; }

    public string? FileType { get; set; }

    public long? FileSizeInBytes { get; set; }

    public string? FileDescription { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    private static Random random = new();

    private int GenerateRandomId()
    {
        // Generates a 6-digit number between 100000 and 999999
        return random.Next(100000, 1000000);
    }

    public TaskDocument()
    {
        Id = GenerateRandomId();
    }
}
