namespace Identity.ViewModels;

public class ExperienceViewModel
{
    public required string Title { get; set; }

    public required DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool PresentlyWorking { get; set; } = false;

    public required string Description { get; set; }
}
