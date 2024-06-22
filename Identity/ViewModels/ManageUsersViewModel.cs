namespace Identity.ViewModels;

public class ManageUsersViewModel
{
	public string? Email { get; set; }

	public Department? Department { get; set; }

	public string? FullName { get; set; }

	public Gender? gender { get; set; }

    public IList<string>? CurrentRoles { get; set; }

}
