using AutoMapper.Execution;

namespace Identity.Controllers;

[Authorize(Roles = "Admin")] // Comment this for first run
public class AdminController(UserManager<Employee> userManager, 
                             RoleManager<IdentityRole> roleManager,
                             ApplicationDBContext dbContext,
                             IMapper mapper) : Controller
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
                return RedirectToAction("ManageRoles");
            else
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
        }
        else
            ModelState.AddModelError("", "Role already exists.");


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
    public async Task<IActionResult> Teams()
    {
        var teams = await dbContext.Teams
                                   .Include(team => team.Manager)
                                   .Include(team => team.Members)
                                   .ToListAsync();
        return View(teams);
    }

    [HttpGet]
    public IActionResult Team(int id)
    {
        var team = dbContext.Teams
                            .Include(team => team.Manager)
                            .Include(team => team.Members)
                            .FirstOrDefault(team => team.Id == id);
        var teamsViewModel = mapper.Map<TeamsViewModel>(team);
        
        return View(teamsViewModel);
    }


    [HttpGet]
    public async Task<IActionResult> CreateTeam()
    {
        var managers = await userManager.GetUsersInRoleAsync("Manager");

        var teamViewModel = new TeamsViewModel
        {
            Members = managers.ToList()
        };


        return View("AddTeam",teamViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromForm]TeamsViewModel request)
    {
        if (!ModelState.IsValid)
        {
            var managers = (List<Employee>)await userManager.GetUsersInRoleAsync("Manager");
            request.Members = managers;
            return View("AddTeam", request);
        }
        var manager = await userManager.FindByEmailAsync(request.Manager);
        if (manager != null)
        {
            var team = new Team()
            {
                Manager = manager,
                Name = request.Name,
                Description = request.Description
            };
            await dbContext.Teams.AddAsync(team);
            await dbContext.SaveChangesAsync();
        }

        return RedirectToAction("Teams");
    }


    [HttpPost]
    public async Task<IActionResult> AddMember(TeamsViewModel request)
    {
        var team = await dbContext.Teams.FirstOrDefaultAsync(team => team.Id == request.Id);      
        var employee = await userManager.FindByEmailAsync(request.Member);
        if (team is null || employee is null)
            return BadRequest();

        if (team.Members.Contains(employee) || !await userManager.IsInRoleAsync(employee, "User"))
            return BadRequest();
        team.Members.Add(employee);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Teams");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMember(TeamsViewModel request)
    {
        var team = await dbContext.Teams.FirstOrDefaultAsync(team => team.Id == request.Id);
        var employee = await userManager.FindByEmailAsync(request.Member);
        if (team is null || employee is null)
            return BadRequest();

        if (!team.Members.Contains(employee))
            return BadRequest();
        team.Members.Remove(employee);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Teams");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTeam([FromForm] int teamId)
    {
        var team = await dbContext.Teams
                                  .Include(team=>team.Members)
                                  .FirstOrDefaultAsync(team => team.Id == teamId);
        if (team is null || team.Members.Count != 0)
            return RedirectToAction("Teams");
        dbContext.Teams.Remove(team);
        await dbContext.SaveChangesAsync();
        return RedirectToAction("Teams");
    }


    [HttpGet]
    public async Task<IActionResult> ViewTeam(int teamId)
    {
        var team = dbContext.Teams
                            .Include(t => t.Members)
                            .ThenInclude(e=>e.Tasks)
                            .FirstOrDefault(t => t.Id == teamId);
        return View(team);
    }

    [HttpGet]
    public async Task<IActionResult> ViewTasks(string email)
    {

        var employee = userManager.Users.Include(e => e.Tasks).FirstOrDefault(e => e.Email == email);

        var tasksViewModel = new List<TaskViewModel>();
        foreach (var task in employee.Tasks)
        {
            tasksViewModel.Add(new TaskViewModel()
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                CreatedDate = task.CreatedDate,
                TargetDate = task.TargetDate,
                Status = task.Status.ToString(),
                Member = email
            });
        }
        ViewBag.Email = employee.Email;
        ViewBag.DepartMent = employee.EmployeeDepartment;
        ViewBag.Gender = employee.EmployeeGender.ToString();
        ViewBag.Name = employee.FirstName + employee.LastName;
        ViewBag.TotalTasks = employee.Tasks.Count;
        return View(tasksViewModel);
    }
}