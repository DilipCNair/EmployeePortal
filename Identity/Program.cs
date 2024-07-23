using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllersWithViews();

var keyVaultEndpoint = new Uri(config["AzureKeyVault"] ?? "");
config.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

builder.Services.AddDbContext<ApplicationDBContext>
                (options => options.UseSqlServer(config["AzureSQL"]));

builder.Services.AddIdentity<Employee, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthorizationBuilder()
                .AddPolicy("EditRolePolicy", policy =>
                                             policy.RequireClaim("EditRole")
                                                   .RequireRole("Admin")
                                                   .RequireAssertion(context =>
                                                                     context.User.IsInRole("Manager")));
//Require assertion is not used when require claim and require role is used
builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddScoped<HttpContextAccessor>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("GoogleSMTP"));

builder.Services.AddTransient<IEmailService, EmailService>();


builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                options =>
                {
                    options.LoginPath = "/Home/Signin";
                });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios,
    // see https://aka.ms/aspnetcore-hsts.
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
