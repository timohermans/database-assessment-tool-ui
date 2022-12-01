using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AuthorizeFolder("/Students", KeyConstants.PolicyForAdminKey);
    options.Conventions.AllowAnonymousToFolder("/Authentication");
    options.Conventions.AllowAnonymousToPage("/Index");
});

#region authentication specific
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(KeyConstants.PolicyForAdminKey,
         policy => policy.RequireRole(KeyConstants.ClaimRoleAdmin));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(cookieOptions => {
    cookieOptions.LoginPath = "/Authentication/Login";
});
builder.Services.AddDataProtection();
builder.Services.AddSingleton<IPasswordProtector, PasswordProtector>();
#endregion

builder.Services.AddScoped<IAssessmentToolDbProvider, AssessmentToolDbProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
