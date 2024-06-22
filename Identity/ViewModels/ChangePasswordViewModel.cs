namespace Identity.ViewModels;

public class ChangePasswordViewModel
{
    [DataType(DataType.Password)]
    public required string OldPassword { get; set; }

    [DataType(DataType.Password)]
    [Remote(action: "ValidatePassword", controller: "Home")]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword { get; set; }
}
