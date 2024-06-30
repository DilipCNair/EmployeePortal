namespace Identity.Controllers;

public class ManagerController(UserManager<Employee> userManager, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await GetUsers();
        return View("ManageEmployees", users);
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployee(string? email)
    {
        if(email is null)
        {
            ModelState.AddModelError("", "Email not provided");
            return View("ManageEmployees", await GetUsers());
        }

        var emp = await userManager.FindByEmailAsync(email);
        if (emp != null)
        {
            var model = mapper.Map<ManagerViewModel>(emp);
            return View("EditEmployee", model);
        }

        ModelState.AddModelError("", "User not found");
        return View("EditEmployee");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(ManagerViewModel model)
    {
        if(!ModelState.IsValid)
            return View("EditEmployee", model);                  

        var emp = await userManager.FindByNameAsync(model.UserName);
        if (emp == null)
        {
            ModelState.AddModelError("","Cannot find the employee");
            return View("EditEmployee", model);
        }

        mapper.Map(model, emp);
        emp.Email = model.UserName;
        var result = await userManager.UpdateAsync(emp);
        if (result.Succeeded)
            return RedirectToAction("Users");
        else
        {
            ModelState.AddModelError("", "Some internal server error occurred");
            return View("EditEmployee", model);
        }
    }

    private async Task<List<BasicDetailsViewModelForManager>> GetUsers()
    {
        var employees = userManager.Users.ToList();

        List<BasicDetailsViewModelForManager> users = [];

        foreach (var employee in employees)
        {
            var roles = await userManager.GetRolesAsync(employee);
            var emp = mapper.Map<BasicDetailsViewModelForManager>(employee);
            emp.Roles = (List<string>?)roles;
            users.Add(emp);
        }

        return users;
    }
}
