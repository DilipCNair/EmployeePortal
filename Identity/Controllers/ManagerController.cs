namespace Identity.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        return View("ManageEmployees");
    }
}
