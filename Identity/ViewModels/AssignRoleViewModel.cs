namespace Identity.ViewModels;

public class AssignRoleViewModel
{
    public string? Username { get;set; }

    public string? Email { get; set; }

    public IList<string>? CurrentRoles { get; set; }

    public IEnumerable<IdentityRole>? AllRoles { get; set; }

    public string? SelectedRole { get; set; }

}
