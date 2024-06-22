namespace Identity.Controllers;

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
        var UserName = User.Identity?.Name;
        Employee? user = new();
        if (UserName is not null)
            user = await userManager.FindByNameAsync(UserName);
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
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

    [HttpPost]
    public async Task<IActionResult> UpdateBasicDetails(BasicDetailsViewModel model)
    {
        //Updates
        if (ModelState.IsValid && User.Identity?.Name is not null)
        {
            var employee = await userManager.FindByNameAsync(User.Identity.Name);
            if (employee != null)
            {                
                mapper.Map(model, employee);
                var result = await userManager.UpdateAsync(employee);
                if (result.Succeeded)
                {
                    TempData["Notification"] = "Your basic details have been updated successfully";
                    return RedirectToAction("Profile");
                }
                else
                    ModelState.AddModelError(string.Empty, "Coundn't update your details now, please try again later");

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
                            .Include(x => x.ProfilePic)
                            .SingleAsync(x => x.Email == email);

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
        var newLocalPath = $"{webHostEnvironment.WebRootPath}\\images\\{newFileName}{newExtension}";
        var newProfilePic = new ProfilePicture()
        {
            FileName = newFileName,
            FileDescription = "The profile pic set by the user",
            FileExtension = newExtension,
            FileSizeInBytes = ProfilePicture?.Length,
            FileType = "unknown",
            FileURI = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}{httpContextAccessor.HttpContext?.Request.PathBase}/images/{newFileName}{newExtension}"
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
            var ExistingLocalPath = $"{webHostEnvironment.WebRootPath}\\images\\{employee.ProfilePic.FileName}{employee.ProfilePic.FileExtension}";
            if (System.IO.File.Exists(ExistingLocalPath))
                System.IO.File.Delete(ExistingLocalPath);
            var profilePicture = dbContext.ProfilePics.FirstOrDefault(pic => pic.Id == employee.ProfilePic.Id);
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
}
