namespace Identity.ViewModels;

public class BasicDetailsViewModelForManager
{
    public string? Email { get; set; }

    public Decimal? Salary { get; set; }

    public string? GovermentId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public Department? EmployeeDepartment { get; set; }

    public Gender? EmployeeGender { get; set; }

    public List<string>? Roles {  get; set; }
}
