namespace Identity.Controllers;

[Authorize]
public class AccountController(ApplicationDBContext dbContext, 
                               HttpContextAccessor httpContextAccessor, 
                               IWebHostEnvironment webHostEnvironment, 
                               IMapper mapper, 
                               UserManager<Employee> userManager, 
                               SignInManager<Employee> signInManager) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Home()
    {
        var email = User.Identity?.Name;
        var employee = await userManager.Users
                            .Include(x => x.Address)
                            .Include(x => x.ProfilePic)
                            .Include(x => x.WorkExperience)
                            .SingleAsync(x => x.Email == email);
        employee.WorkExperience = employee?.WorkExperience?
                                  .OrderByDescending(exp => exp.PresentlyWorking)
                                  .ThenByDescending(exp => exp.EndDate)
                                  .ToList();
        return View(employee);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        //Update
        var email = User?.Identity?.Name;
        if(email != null)
        {
            var employee = await userManager.Users
                            .Include(x => x.Address)
                            .Include(x => x.ProfilePic)
                            .SingleAsync(x => x.Email == email);

            if (employee != null)
            {
                var emp = mapper.Map<ProfileViewModel>(employee);
                emp.EmployeeAddress = mapper.Map<AddressViewModel>(employee.Address);
                emp.ProfilePic = employee.ProfilePic?.FileURI;
                ViewBag.Notification = TempData["Notification"]?.ToString();
                return View(emp);
            }
            return RedirectToAction("Error", "Home");
        }
        return RedirectToAction("Error","Home");    
        
    }

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var email = User?.Identity?.Name;
        if (email is null)
            return BadRequest("You are not logged in");

        var emp = await userManager.FindByEmailAsync(email);
        if (emp is null)
            return BadRequest("Some internal server error");

        var model = mapper.Map<ProfileViewModel>(emp);        
        return View("Details",model);
    }

    [HttpPost]
    public async Task<IActionResult> Summary(ProfileViewModel request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var email = User?.Identity?.Name;
        if (email is null)
            return BadRequest("You are not logged in");

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
            return BadRequest("Some internal server error");

        employee.Summary = request.Summary;
        var result = await userManager.UpdateAsync(employee);
        if (result.Succeeded)
            return RedirectToAction("Details");
        else
            return BadRequest("Some internal server error");

    }

    [HttpPost]
    public async Task<IActionResult> Experience(ProfileViewModel request)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (request.Experience is null)
            return BadRequest();

        var email = User?.Identity?.Name;
        if (email is null)
            return BadRequest("User not found");

        var employee = await userManager.Users
                            .Include(x => x.WorkExperience)
                            .SingleAsync(x => x.Email == email);
        if (employee == null)
            return BadRequest("Server error");

        var experience = mapper.Map<Experience>(request.Experience);

        employee.WorkExperience ??= [];
        employee.WorkExperience.Add(experience);       
        
        var result = await userManager.UpdateAsync(employee);
        if (result.Succeeded)
            return RedirectToAction("Details");
        else
        {
            TempData["Error"] = "Cannot Update the user";
            return RedirectToAction("Details");
        }
    }

    [HttpPost]
    public async Task<IActionResult> RemoveExperience(Guid? Id)
    {
        if (!ModelState.IsValid)
            return BadRequest();
       

        var email = User?.Identity?.Name;
        if (email is null)
            return BadRequest("User not found");

        var employee = await userManager.Users
                            .Include(x => x.WorkExperience)
                            .SingleAsync(x => x.Email == email);
        if (employee == null)
            return BadRequest("Server error");

        var experience = employee?.WorkExperience?.FirstOrDefault(exp => exp.Id == Id);
        if (experience != null)
            employee?.WorkExperience?.Remove(experience);

        var result = await userManager.UpdateAsync(employee);

        if (result.Succeeded)
            return RedirectToAction("Home");
        else
        {
            TempData["Error"] = "Cannot Update the user";
            return RedirectToAction("Home");
        }
    }


    [HttpPost]
    public async Task<IActionResult> UpdateBasicDetails(BasicDetailsViewModel request)
    {
        //Updates basic details
        if (ModelState.IsValid && User.Identity?.Name is not null)
        {
            var employee = await userManager.FindByNameAsync(User.Identity.Name);
            if (employee != null)
            {
                request.EmployeeDepartMent = employee.EmployeeDepartment;
                mapper.Map(request, employee);
                var result = await userManager.UpdateAsync(employee);
                if (result.Succeeded)
                {
                    TempData["Notification"] = "Your basic details have been updated successfully";
                    return RedirectToAction("Profile");
                }
                else
                    ModelState.AddModelError(string.Empty, "Coundn't update your details now, " +
                        "                                           please try again later");

                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User Not Found");
                return RedirectToAction("Profile");
            }
        }
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAddress(AddressViewModel model)
    {
        if (ModelState.IsValid && User.Identity?.Name is not null)
        {
            var email = User?.Identity?.Name;
            if (email == null)
                return RedirectToAction("Profile");

            var employee = await userManager.FindByEmailAsync(email);
            if (employee is not null)
            {
                employee.Address ??= new();
                mapper.Map(model, employee.Address);
                
                var result = await userManager.UpdateAsync(employee);
                if (result.Succeeded)
                {
                    TempData["Notification"] = "Your address has been updated successfully";
                    return RedirectToAction("Profile");
                }
                else
                    TempData["Error"]= "Coundn't update your details now,please try again later";

                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User Not Found");
                return RedirectToAction("Profile");
            }
        }
        return RedirectToAction("Profile");
    }

    [HttpGet]
    public async Task<IActionResult> Picture()
    {
        var email = User?.Identity?.Name;
        if (email != null)
        {
            var user = await userManager.FindByNameAsync(email);
            var employee = await userManager.Users
                            .Include(emp => emp.ProfilePic)
                            .SingleAsync(emp => emp.Email == email);

            if (employee != null)
            {
                return View("Picture", employee?.ProfilePic?.FileURI);
            }
        }
        return RedirectToAction("Error", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePicture(IFormFile ProfilePicture)
    {
        var email = User?.Identity?.Name;

        if (email is null || ProfilePicture is null)
        {
            TempData["Error"] = "Email or form data is null";
            return RedirectToAction("Picture");
        }
            
        if (!IsValidProfilePicUpload(ProfilePicture))
        {
            TempData["Error"] = "Profile Picture not valid";
            return RedirectToAction("Picture");
        }

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
        {
            TempData["Error"] = "User not found";
            return RedirectToAction("Picture");
        }

        //Explicit loading of profile pic navigation property
        dbContext.Entry(employee).Reference(emp => emp.ProfilePic).Load();

        //Checks if employee has a profile pic already, if yes it removes it first
        if (employee.ProfilePic != null)
            await RemovePicture();

        //Updates the profile pic
        var newFileName = Guid.NewGuid().ToString();
        var newExtension = Path.GetExtension(ProfilePicture.FileName);
        var newLocalPath = $"{webHostEnvironment.WebRootPath}" +
                           $"//images//{newFileName}{newExtension}";
        var newProfilePic = new ProfilePicture()
        {
            FileName = newFileName,
            FileDescription = "The profile pic set by the user",
            FileExtension = newExtension,
            FileSizeInBytes = ProfilePicture?.Length,
            FileType = "unknown",
            FileURI = $"{httpContextAccessor.HttpContext?.Request.Scheme}:" +
                      $"//{httpContextAccessor.HttpContext?.Request.Host}" +
                      $"{httpContextAccessor.HttpContext?.Request.PathBase}" +
                      $"/images/{newFileName}{newExtension}"
        };
        employee.ProfilePic = newProfilePic;
        if (ProfilePicture is null)
        {
            TempData["Error"] = "Profile Picture not provided";
            return RedirectToAction("Picture");
        }

        using var stream = new FileStream(newLocalPath, FileMode.Create);
        await ProfilePicture.CopyToAsync(stream);

        var result = await userManager.UpdateAsync(employee);
        if (result.Succeeded)
        {
            TempData["Notification"] = "Your profile picture has been updated successfully";
            return RedirectToAction("Profile");
        }
        else
        {
            TempData["Error"] = "Some internal server error occured";
            return RedirectToAction("Picture");
        }
    }

    private bool IsValidProfilePicUpload(IFormFile ProfilePicture)
    {
        var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(Path.GetExtension(ProfilePicture?.FileName)))
        {
            ModelState.AddModelError("File", "Unsupported file extension");
            return false;
        }

        if (ProfilePicture?.Length > 10485760)
        {
            ModelState.AddModelError("File", "File size more than 10MB, please upload a smaller size file.");
            return false;
        }

        return true;
    }

    [HttpPost]
    public async Task<IActionResult> RemovePicture()
    {
        var email = User?.Identity?.Name;

        if (email is null)
        {
            TempData["Error"] = "User not authenticated";
            return RedirectToAction("Profile");
        }

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
        {
            TempData["Error"] = "User not found";
            return RedirectToAction("Profile");
        }

        //Exmplicit loading of profile pic navigation porperty
        dbContext.Entry(employee).Reference(emp => emp.ProfilePic).Load();

        //Checks if employee has a profile pic already, if yes it removes it first
        if (employee.ProfilePic != null)
        {
            var ExistingLocalPath = $"{webHostEnvironment.WebRootPath}//images//" +
                $"{employee.ProfilePic.FileName}{employee.ProfilePic.FileExtension}";
            if (System.IO.File.Exists(ExistingLocalPath))
                System.IO.File.Delete(ExistingLocalPath);
            var profilePicture = dbContext.ProfilePics.FirstOrDefault
                                (pic => pic.Id == employee.ProfilePic.Id);
            if (profilePicture != null)
            {
                dbContext.ProfilePics.Remove(profilePicture);
                await dbContext.SaveChangesAsync();
                TempData["Notification"] = "Your profile picture has been removed successfully";
                return RedirectToAction("Profile");
            }
        }
        TempData["Error"] = "No profile picture found";
        return RedirectToAction("Profile");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var currentPassword = changePasswordViewModel.OldPassword;
        var newPassword = changePasswordViewModel.Password;

        if(email is null)
        {
            ModelState.AddModelError("", "bad request");
            return View("ChangePassword");
        }

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "bad request");
            return View("ChangePassword");
        }

        if (string.Compare(currentPassword, newPassword) == 0)
        {
            ModelState.AddModelError("", "New password is same as old password");
            return View("ChangePassword");
        }

        var employee = await userManager.FindByEmailAsync(email);
        if (employee == null)
        {
            ModelState.AddModelError("", "User not found");
            return View("ChangePassword");
        }

        var result = await userManager.ChangePasswordAsync(employee, currentPassword, newPassword);
        if (result.Succeeded)
        {
            TempData["Notification"] = "Your password has been updated successfully";
            return RedirectToAction("Profile");
        }
        else
        {
            ModelState.AddModelError("", "Old password is wrong");
            return View("ChangePassword");
        }

    }

    [HttpGet]
    public IActionResult ConfirmDelete()
    {
        return View("ConfirmDelete");
    }

    [HttpPost]
    public async Task<IActionResult> Delete()
    {
        if (User.Identity?.Name is not null)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user is not null)
            {
                await signInManager.SignOutAsync();
                await RemovePicture();
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Deleted","Home");
                else
                    return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Error", "Home");
        }
        return RedirectToAction("Error","Home");
    }

    [HttpPost]
    public async Task<IActionResult> Signout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Signin","Home");
    }

    [HttpGet]
    public IActionResult Tasks()
    {
        string email = User.Identity.Name;
        var employee = dbContext.Users
                             .Include(emp => emp.Tasks)
                             .FirstOrDefault(emp => emp.Email == email);
        if (employee == null)
            return View();

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

    [HttpGet]
    public IActionResult Task(int id)
    {
        var task = dbContext.EmployeeTasks
                            .Include(t=>t.Notes)
                            .Include(t => t.TaskDocuments)
                            .FirstOrDefault(task => task.Id == id);
        if (task is null)
            return BadRequest();
        var taskViewModel = mapper.Map<TaskViewModel>(task);
        taskViewModel.TaskStatus = task.Status;


        return View(taskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int taskId, string status)
    {
        var task = dbContext.EmployeeTasks.FirstOrDefault(task => task.Id == taskId);
        if (task is null)
            return BadRequest();
        if (System.Enum.TryParse(status, out EmployeeTaskStatus taskStatus))
            task.Status = taskStatus;
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotes(int taskId)
    {
        var email = User?.Identity?.Name;

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
            return BadRequest();

        var task = dbContext.EmployeeTasks
                            .Include(e => e.Notes)
                            .FirstOrDefault(task => task.Id == taskId);
        if (task is null)
            return BadRequest();

        return View(task.Notes);
    }

    
    [HttpPost]
    public async Task<IActionResult> AddNote(int id, string newNote, string name)
    {
        var email = User?.Identity?.Name;

        var employee = dbContext.Users
                                .Include(e=>e.Tasks)
                                .ThenInclude(t=>t.Notes)
                                .FirstOrDefault(e=>e.Email==email);
                
        if (employee is null)
            return BadRequest();

        var task = dbContext.EmployeeTasks
                            .Include(e=>e.Notes)
                            .FirstOrDefault(task => task.Id == id);
        if (task is null)
            return BadRequest();


        var note = new TaskNote()
        {
            Note = newNote,
            Name = name
        };

        dbContext.TaskNotes.Add(note);
        task.Notes.Add(note);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Task",new { id });
    }


    [HttpPost]
    public async Task<IActionResult> DeleteNote(int taskId, int noteId)
    {
        var email = User?.Identity?.Name;

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
            return RedirectToAction("Task", new { id = taskId });

        var task = dbContext.EmployeeTasks
                            .Include(e => e.Notes)
                            .FirstOrDefault(task => task.Id == taskId);
        if (task is null)
            return RedirectToAction("Task", new { id = taskId });


        var note = dbContext.TaskNotes.FirstOrDefault(n => n.Id == noteId);
        if (note is null)
            return RedirectToAction("Task", new { id = taskId });

        dbContext.TaskNotes.Remove(note);
        task.Notes.Remove(note);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Task", new { id = taskId });
    }


    [HttpPost]
    public async Task<IActionResult> AddDocument(int taskId, IFormFile document, string description)
    {
        var email = User?.Identity?.Name;

        if (email is null || document is null)
        {
            TempData["Error"] = "Email or form data is null";
            return RedirectToAction("Tasks");
        }

        if (!IsValidDocument(document))
        {
            TempData["Error"] = "document not valid";
            return RedirectToAction("Tasks");
        }

        var employee = await userManager.FindByEmailAsync(email);
        if (employee is null)
        {
            TempData["Error"] = "User not found";
            return RedirectToAction("Tasks");
        }

        var newFileName = document.FileName;
        var newExtension = Path.GetExtension(document.FileName);
        var newLocalPath = $"{webHostEnvironment.WebRootPath}" +
                           $"\\documents\\{newFileName}";
        var newDocument = new TaskDocument()
        {
            FileName = newFileName,
            FileDescription = description,
            FileExtension = newExtension,
            FileSizeInBytes = document?.Length,
            FileType = "unknown",
            FileURI = $"{httpContextAccessor.HttpContext?.Request.Scheme}:" +
                      $"//{httpContextAccessor.HttpContext?.Request.Host}" +
                      $"{httpContextAccessor.HttpContext?.Request.PathBase}" +
                      $"/documents/{newFileName}"
        };

        var task = dbContext.EmployeeTasks
                            .Include(e => e.TaskDocuments)
                            .FirstOrDefault(task => task.Id == taskId);

        dbContext.TaskDocuments.Add(newDocument);
        task.TaskDocuments.Add(newDocument);

        using var stream = new FileStream(newLocalPath, FileMode.Create);
        await document.CopyToAsync(stream);

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Task", new {id=taskId});
    }


    [HttpPost]
    public async Task<IActionResult> RemoveDocument(int taskId, int documentId)
    {
        var task = dbContext.EmployeeTasks
                           .Include(e => e.TaskDocuments)
                           .FirstOrDefault(task => task.Id == taskId);

        var document = dbContext.TaskDocuments
                                .FirstOrDefault(d => d.Id == documentId);

        var path = $"{webHostEnvironment.WebRootPath}\\documents\\" +
                   $"{document.FileName}{document.FileExtension}";
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        dbContext.TaskDocuments.Remove(document);
        task.TaskDocuments.Remove(document);
        await dbContext.SaveChangesAsync();

        return RedirectToAction("Task", new { id = taskId });
    }

    private bool IsValidDocument(IFormFile document)
    {
        var allowedExtensions = new string[] {".pdf",".docx",".doc",".odt",".ppt",".pptx",".txt"};

        if (!allowedExtensions.Contains(Path.GetExtension(document?.FileName)))
        {
            ModelState.AddModelError("File", "Unsupported file extension");
            return false;
        }

        if (document?.Length > 10485760)
        {
            ModelState.AddModelError("File", "File size more than 10MB, please upload a smaller size file.");
            return false;
        }

        return true;
    }
}
