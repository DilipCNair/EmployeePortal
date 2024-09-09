var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(options => 
                options.UseNpgsql(config.GetConnectionString("PostgreSQL")));

builder.Services.AddIdentity<Employee, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDBContext>();


builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddScoped<HttpContextAccessor>();

builder.Services.Configure<MailSettings>(config.GetSection("GoogleSMTP"));

builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.PostConfigure<CookieAuthenticationOptions>
                            (IdentityConstants.ApplicationScheme,
                            options =>{options.LoginPath = "/Home/Signin";});


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
