namespace Identity.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> ManageUsers()
    {
        var manageUsersViewModel = new List<ManageUsersViewModel>();
        var users = await userManager.Users.ToListAsync();


        foreach(var user in users)
        {            
            var currentRoles = await userManager.GetRolesAsync(user);
            manageUsersViewModel.Add(new ManageUsersViewModel
            {
                Email = user.Email,
                Department = user.EmployeeDepartment,
                FullName = user.FirstName + " " + user.MiddleName + " " + user.LastName,
                gender = user.EmployeeGender,
                CurrentRoles = currentRoles
            });
        }
		return View(manageUsersViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ManageRoles()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return View(roles);
    }
    
    public IActionResult RemoveRoles(string email)
    {
        ViewBag.Email = email;
        return View();
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpGet]
    public IActionResult UpdateRole(string role)
    {
        return View("UpdateRole",role);
    }

    [HttpGet]
    public IActionResult DeleteRole(string role)
    {
        return View("DeleteRoleConfirmation",role);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {

        roleName = roleName.Trim().Replace(" ", "");
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var newRole = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                // Role created successfully
                return RedirectToAction("ManageRoles");
            }
            else
            {
                // Handle errors if any
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        else
        {
            // Role already exists
            ModelState.AddModelError("", "Role already exists.");
        }

        // If we reach here, something went wrong, redisplay the form
        return View("CreateRole");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(string currentRoleName, string newRoleName)
    {
        var existingRole = await roleManager.FindByNameAsync(currentRoleName);

        if (existingRole != null)
        {
            existingRole.Name = newRoleName;
            var result = await roleManager.UpdateAsync(existingRole);

            if (result.Succeeded)
            {
                // Role updated successfully
                return RedirectToAction("ManageRoles");
            }
            else
            {
                // Handle errors if any
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        else
        {
            // Role not found
            ModelState.AddModelError("", "Role not found.");
        }

        // If we reach here, something went wrong, redisplay the form
        return View("UpdateRole");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRole()
    {
        string? roleName = Request.Form["roleName"];
        if (!string.IsNullOrEmpty(roleName))
        {
            var roleToDelete = await roleManager.FindByNameAsync(roleName);

            if (roleToDelete != null)
            {
                var result = await roleManager.DeleteAsync(roleToDelete);

                if (result.Succeeded)
                    return RedirectToAction("ManageRoles");
                else
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
            }
            else
                ModelState.AddModelError("", "Role not found");
        }
        return RedirectToAction("Error", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> AssignRoles(string email)
    {
        var user = await userManager.FindByNameAsync(email);
        var roles = await userManager.GetRolesAsync(user);

        var assignRoleViewModel = new AssignRoleViewModel
        {
            Username = email,
            Email = email,
            CurrentRoles = roles,
            AllRoles = roleManager.Roles.ToList()

        };

        return View(assignRoleViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToRole(AssignRoleViewModel assignRoleViewModel)
    {
        assignRoleViewModel.Username = assignRoleViewModel.Email;

        var userName = assignRoleViewModel.Email;
        var roleName = assignRoleViewModel.SelectedRole;

        Employee? user;
        IdentityRole? role;

        if (userName != null && roleName != null)
        {
            user = await userManager.FindByNameAsync(userName);
            role = await roleManager.FindByNameAsync(roleName);

            if (user != null && role?.Name != null )
            {
                if (await userManager.IsInRoleAsync(user, roleName))
                {
                    assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
                    assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
                    ModelState.AddModelError("", "User is already in " + roleName + " role");
                    return View("AssignRoles", assignRoleViewModel);
                }

                var result = await userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    // User associated with role successfully
                    assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
                    assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
                    return View("AssignRoles", assignRoleViewModel);
                }
                else
                {
                    // Handle errors if any
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
                ModelState.AddModelError("", "User or role not found.");
        }

        // If we reach here, something went wrong, redisplay the form
        user = await userManager.FindByNameAsync(userName);
        assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
        assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
        return View("AssignRoles", assignRoleViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> RemoveUserFromRole(AssignRoleViewModel assignRoleViewModel)
    {
        assignRoleViewModel.Username = assignRoleViewModel.Email;
        var userName = assignRoleViewModel.Email;
        var roleName = assignRoleViewModel.SelectedRole;

        Employee? user;
        IdentityRole? role;
        if (userName != null && roleName != null)
        {
            user = await userManager.FindByNameAsync(userName);
            role = await roleManager.FindByNameAsync(roleName);

            if (user != null && role != null)
            {
                if (!await userManager.IsInRoleAsync(user, roleName))
                {
                    assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
                    assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
                    ModelState.AddModelError("", "User is not in " + roleName + " role");
                    return View("AssignRoles", assignRoleViewModel);
                }

                var result = await userManager.RemoveFromRoleAsync(user, role.Name);

                if (result.Succeeded)
                {
                    // User associated with role successfully
                    assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
                    assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
                    return View("AssignRoles", assignRoleViewModel);
                }
                else
                {
                    // Handle errors if any
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
        }

        else
        {
            // User or role not found
            ModelState.AddModelError("", "User or role not found.");
        }

        // If we reach here, something went wrong, redisplay the form
        user = await userManager.FindByNameAsync(userName);
        assignRoleViewModel.CurrentRoles = (IList<string>?)await userManager.GetRolesAsync(user);
        assignRoleViewModel.AllRoles = roleManager.Roles.ToList();
        return View("AssignRoles", assignRoleViewModel);
    }


    [HttpGet]
    public async Task<IActionResult> ManageClaims(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
        {
            ModelState.AddModelError("", $"User with email = {email} cannot be found");
            return View();
        }
        var model = new UserClaimsViewModel
        {
            Email = email
        };
        var existingUserClaims = await userManager.GetClaimsAsync(user);
        foreach(Claim claim in ClaimsStore.AllClaims)
        {
            var userClaim = new UserClaim()
            {
                User_Claim = new Claim(claim.Type,claim.Value)
            };

            if (existingUserClaims.Any(c=>c.Type == claim.Type))
                userClaim.IsSeleted = true;
            
            model?.UserClaims?.Add(userClaim);
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
    {
        var user = await userManager.FindByIdAsync(model.Email);

        if (user == null)
        {
            ModelState.AddModelError("", $"User with email = {model.Email} cannot be found");
            return View();
        }


        var claims = await userManager.GetClaimsAsync(user);
        var result = await userManager.RemoveClaimsAsync(user, claims);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove claims");
            return View(model);
        }

        foreach (var userClaim in model.UserClaims)
        {
            if (userClaim.IsSeleted)
                result = await userManager.AddClaimAsync(user, userClaim.User_Claim);
        }

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add claims to the user");
            return View(model);
        }

        return View();

    }

}