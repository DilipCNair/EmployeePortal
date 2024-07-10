namespace Identity.ViewModels;

public class ProfileViewModel
{
    public Decimal? Salary { get; set; }

    public string? GovermentId { get; set; }

    public string? FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; } = string.Empty;

    public Department? EmployeeDepartMent { get; set; }

    public Gender? EmployeeGender { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    public string? ProfilePic { get; set; }

    public string? Summary { get; set; }

    public AddressViewModel? EmployeeAddress { get; set; }

    public ExperienceViewModel? Experience { get; set; }

}
