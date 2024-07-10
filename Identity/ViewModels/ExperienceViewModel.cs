namespace Identity.ViewModels;

public class ExperienceViewModel
{
    //public Guid Id { get; set; }

    public string? Title { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool PresentlyWorking { get; set; } = false;

    public string? Description { get; set; }
}
