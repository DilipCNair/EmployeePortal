namespace Identity.ViewModels;

public class UpdateTaskViewModel
{
    public int Id;

    public DateTime DateCompleted { get; set; }

    public EmployeeTaskStatus Status { get; set; }

    public string? Note { get; set; }
}
