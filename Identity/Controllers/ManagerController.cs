namespace Identity.Controllers;


[Authorize(Roles ="Manager")]
public class ManagerController(UserManager<Employee> userManager, 
                               ApplicationDBContext dbContext, 
                               IMapper mapper) : Controller
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

    [HttpGet]
    public IActionResult MyTeam()
    {
        var myTeam = dbContext.Teams
                              .Include(team=>team.Members)
                              .ThenInclude(member => member.Tasks)
                              .FirstOrDefault(team => team.Manager.Email == User.Identity.Name);

        var teamsViewModel = new TeamsViewModel()
        {
            Id = myTeam.Id,
            Name = myTeam.Name,
            Description = myTeam.Description,
            Members = myTeam.Members
        };

        return View(teamsViewModel);
    }

    [HttpGet]
    public IActionResult Tasks(string email)
    {
        var employee = userManager.Users
                                  .Include(emp => emp.Tasks)
                                  .FirstOrDefault(e => e.Email == email);
        if (employee is null)
            return BadRequest();

        var tasksViewModel = new List<TaskViewModel>();
        foreach(var task in employee.Tasks)
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

    [HttpGet]
    public IActionResult CreateTask()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskViewModel request)
    {
        if(!ModelState.IsValid)
            return View(request);

        var task = new EmployeeTask() 
        {
            Description = request.Description,
            Name = request.Name,
            TargetDate = request.TargetDate,
            CreatedDate = DateTime.Now,
            Status = EmployeeTaskStatus.Assigned
        };

        var employee = userManager.Users
                                  .Include(emp => emp.Tasks)
                                  .FirstOrDefault(e => e.Email == request.Member);
        if (employee == null || !await userManager.IsInRoleAsync(employee, "User"))
            return View(request);

        employee.Tasks ??= [];

        //employee.Tasks.Add(task);

        dbContext.EmployeeTasks.Add(task);
        employee.Tasks.Add(task);      
        await userManager.UpdateAsync(employee);
        await dbContext.SaveChangesAsync();

        ViewBag.Email = employee.Email;
        ViewBag.DepartMent = employee.EmployeeDepartment;
        ViewBag.Gender = employee.EmployeeGender.ToString();
        ViewBag.Name = employee.FirstName + employee.LastName;
        ViewBag.TotalTasks = employee.Tasks.Count;

        return RedirectToAction("Tasks", new { email = employee.Email });

    }

    public async Task<IActionResult> RemoveTask(int id)
    {
        var task = dbContext.EmployeeTasks.FirstOrDefault(task => task.Id == id);
        if (task is null)
            return View();
        dbContext.EmployeeTasks.Remove(task);   
        await dbContext.SaveChangesAsync();
        return View();
    }

    [HttpGet]
    public IActionResult GetTask(int taskId)
    {
        var task = dbContext.EmployeeTasks
                            .Include(t=>t.TaskDocuments)
                            .FirstOrDefault(t => t.Id == taskId);

        return View("Documents", task);
    }

    
}
