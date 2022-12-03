using Dapper;
using DatabaseAssessmentTool.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Security.Claims;

namespace DatabaseAssessmentTool.Web.Pages.Authentication;

public class LoginModel : PageModel
{
    private readonly IAssessmentToolDbProvider _dbProvider;
    private readonly IPasswordProtector _protector;

    [Required]
    [BindProperty]
    public required string Username { get; set; }
    [Required]
    [BindProperty]
    public required string Password { get; set; }
    public string? ErrorMessage { get; set; }

    public LoginModel(IAssessmentToolDbProvider dbProvider, IPasswordProtector protector)
    {
        _dbProvider = dbProvider;
        _protector = protector;
    }

    public void OnGet()
    {
    }

    /**
     *  based on https://www.mikesdotnetting.com/article/335/simple-authentication-in-razor-pages-without-a-database
     */
    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        _dbProvider.UpdateCredentials(Username, Password);
        using var db = _dbProvider.Provide();

        ErrorMessage = await TryLogInAsync(db);
        if (!string.IsNullOrEmpty(ErrorMessage)) return Page();
        var isAdmin = await GetIsAdminAsync(db);
        var claims = CreateClaims(isAdmin);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return Redirect(returnUrl ?? (isAdmin ? "/" : "/assignments"));
    }

    private static async Task<bool> GetIsAdminAsync(SqlConnection connection)
    {
        var adminTables = await connection.QueryAsync("SELECT * FROM DBExpert.INFORMATION_SCHEMA.TABLES;");
        var isAdmin = adminTables.Any();
        return isAdmin;
    }

    private async Task<string?> TryLogInAsync(SqlConnection connection)
    {
        try
        {
            var result = await connection.QueryAsync("use DbExpert; exec MyAssignments");
        }
        catch (SqlException error)
        {
            if (error.Message.Contains("Login failed"))
            {
                return "Login failed. Check your username and password.";
            }
            else
            {
                return "Something went wrong. Please contact Gleb.";
            }
        }

        return null;
    }
    private List<Claim> CreateClaims(bool isAdmin)
    {
        var protectedPassword = _protector.Protect(Password);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Username),
            new Claim(ClaimTypes.Role, isAdmin ? KeyConstants.ClaimRoleAdmin : "user"),
            new Claim(KeyConstants.ClaimKeyDatabasePassword, protectedPassword)
        };
        return claims;
    }

}
