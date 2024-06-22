using System.Security.Claims;

namespace Identity.ViewModels;

public class UserClaimsViewModel
{
    public string? Email { get; set; }

    public List<UserClaim>? UserClaims { get; set; }

    public UserClaimsViewModel()
    {
        UserClaims = new List<UserClaim>();
    }
}

public class UserClaim
{
    public Claim? User_Claim { get; set; }

    public bool IsSeleted { get; set; }

}
