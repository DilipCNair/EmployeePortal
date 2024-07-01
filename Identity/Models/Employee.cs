namespace Identity.Models;

public class Employee : IdentityUser
{
    public long EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }
  
    public Department? EmployeeDepartment { get; set; }

    public string? EmpDesignation { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public Decimal? Salary { get; set; }   

    public string? OfficeLocation { get; set; }

    public string? Summary { get; set; }

    public string? GovermentId { get; set; }

    public Gender? EmployeeGender { get; set; }

    public string? CountryCode { get; set; }

    public string? MobileNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public Address? Address { get; set; }

    public ProfilePicture? ProfilePic { get; set; }
    
    public List<Experience>? WorkExperience { get; set;}
    
}
