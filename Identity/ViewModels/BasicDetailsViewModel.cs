namespace Identity.ViewModels;

public class BasicDetailsViewModel
{
    public Decimal? Salary { get; set; }

    public string? GovermentId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public Department? EmployeeDepartMent { get; set; }

    public Gender? EmployeeGender { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
}
