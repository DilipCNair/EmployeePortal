namespace Identity.Models;

public class Experience
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? PresentlyWorking { get; set; }

    public string? Description { get; set; }    
}
