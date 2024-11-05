namespace Identity.ViewModels;

public class TaskViewModel
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime TargetDate { get; set; } = DateTime.UtcNow;

    public string Member { get;set; }

    public string Status { get; set; } = "Assigned";

    public EmployeeTaskStatus TaskStatus { get; set; }

    public List<TaskNote> Notes { get; set; } = [];

    public List<TaskDocument> TaskDocuments { get; set; } = [];

    public bool DetailsSelected { get; set; } = true;

    public bool NotesSelected { get; set; } = false;

    public bool DocumentsSelected { get; set; } = false;

}
