namespace Identity.Controllers;

[AllowAnonymous]
public class HomeController(UserManager<Employee> userManager, 
                            SignInManager<Employee> signInManager,
                            IEmailService email,
                            HttpContextAccessor httpContextAccessor) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (!signInManager.IsSignedIn(User))
            return View();
        else
            return RedirectToAction("Home","Account");
    }

    [HttpGet]
    public IActionResult About()
    {
        var model = new ProfileViewModel();
        return View("About", model);
    }

    [HttpGet]
    public IActionResult Contact()
    {
        var model = new ProfileViewModel();
        return View("Contact", model);
    }


    [HttpGet]
    public IActionResult Signin()
    {      
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Signin(SigninViewModel model)
    {
        if(ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email,model.Password,
                                                                 model.RememberMe,false);
            if (result.Succeeded)
            {
                var employee = await userManager.Users
                                .Include(x => x.ProfilePic)
                                .SingleAsync(x => x.Email == model.Email);
                if (employee != null)
                {
                    var oldClaims = await userManager.GetClaimsAsync(employee);
                    var firstName = oldClaims.FirstOrDefault(c => c.Type == "FirstName");
                    var profilePicturePath = oldClaims.FirstOrDefault(c => c.Type == "ProfilePicturePath");
                    if (firstName != null)
                        await userManager.RemoveClaimAsync(employee, firstName);
                    if (profilePicturePath != null)
                        await userManager.RemoveClaimAsync(employee, profilePicturePath);

                    var claims = new List<Claim>
                    {
                        new("ProfilePicturePath", employee?.ProfilePic?.FileURI ?? ""),
                        new("FirstName", employee?.FirstName ?? string.Empty)
                    };
                    await userManager.AddClaimsAsync(employee, claims);
                    return RedirectToAction("Home", "Account");
                }               
            }

            ModelState.AddModelError(string.Empty, "Invalid Credentials");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View("ForgotPassword");
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View("ForgotPassword");

        var employee = await userManager.FindByEmailAsync(model.Email);

        if (employee != null)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(employee);
            var url = $"{httpContextAccessor.HttpContext?.Request.Scheme}:" +
                      $"//{httpContextAccessor.HttpContext?.Request.Host}" +
                      $"{httpContextAccessor.HttpContext?.Request.PathBase}" +
                      $"/Home/ResetPassword?email={model.Email}&token={token}";


            MailData mailData = new()
            {
                EmailToId = model.Email,
                EmailToName = "Dilip",
                EmailSubject = "Password Reset",
                EmailBody = $"\nDear {(employee.FirstName??"User")}\n\n" +
                            $"Click on the below link to reset your password\n\n{url}"
            };


            var result = await email.SendMailAsync(mailData);
            if (result)
                return View("ForgotPasswordMessage");
            else
                return BadRequest("Error");
        }

        return BadRequest("Error");
    }

    [HttpGet]
    public IActionResult ResetPassword(string? email, string? token)
    {
        ResetPasswordViewModel model = new()
        {
            Email = email,
            Token = token,
            Password = null,
            ConfirmPassword = null,

        };
        return View("ResetPassword",model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) 
           return View("ResetPassword", model);

        model.Token = model.Token.Replace(" ", "+");

        var employee = await userManager.FindByEmailAsync(model.Email);
        if (employee is null)
            return BadRequest("Employee not found");

        var result = await userManager.ResetPasswordAsync(employee, model.Token, model.Password);
        if (result.Succeeded)
        {
            MailData mailData = new()
            {
                EmailToId = model.Email,
                EmailToName = "Dilip",
                EmailSubject = "Password Reset Successful",
                EmailBody = $"\nDear {(employee.FirstName ?? "User")}\n\n" +
                            $"Your password has been changed successfully."
            };

            await email.SendMailAsync(mailData);
            return View("ResetPasswordSuccess");
        }
        else
            return BadRequest("Reset password failed");
    }

    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ValidateEmail(string Email)
    {
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        Regex regex = new(pattern);
        if (!regex.IsMatch(Email))
            return Json("Email is not in valid format");
        var user = await userManager.FindByEmailAsync(Email);
        if (user is null)
            return Json(true);
        else
            return Json("This Email is already in use");
    }

    [HttpGet]
    public IActionResult ValidatePassword(string Password)
    {
        string message = "Password should be of minimum 8 characters, it should be a combination of " +
                         "upper case, lower case, numbers, special characters and must not " +
                         "contain white spaces";
        
        // Check minimum length
        if (Password.Length < 8)
            return Json(message);

        // Check for special character
        if (!Regex.IsMatch(Password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
            return Json(message);

        // Check for uppercase letter
        if (!Regex.IsMatch(Password, @"[A-Z]"))
            return Json(message);

        // Check for lowercase letter
        if (!Regex.IsMatch(Password, @"[a-z]"))
            return Json(message);

        // Check for digit
        if (!Regex.IsMatch(Password, @"\d"))
            return Json(message);

        // Check for whitespace
        if (Password.Contains(" "))
            return Json(message);

        return Json(true);
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if(ModelState.IsValid)
        {          
            var user = new Employee
            {
                UserName = model.Email,
                Email = model.Email,            
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Home","Account");
            }
            ModelState.AddModelError(string.Empty, "Some internal server error occured");
        }
        return View(model);
    }

    public IActionResult Deleted()
    {
        return View();
    }

  
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}