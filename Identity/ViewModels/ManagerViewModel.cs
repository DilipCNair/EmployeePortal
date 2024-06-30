namespace Identity.ViewModels;

public class ManagerViewModel
{
    public required string UserName { get; set; }

    public string? Email { get; set; }

    public string? EmpDesignation { get; set; }

    public Department? EmployeeDepartment { get; set; }

    public string? OfficeLocation { get; set; }

    [Column(TypeName = "decimal(18, 4)")]
    public Decimal? Salary { get; set; }
}
