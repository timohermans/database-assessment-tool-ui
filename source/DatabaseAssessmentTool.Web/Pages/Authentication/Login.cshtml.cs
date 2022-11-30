using Dapper;
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
    public class Request
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }

    private readonly string _databaseUrl;

    [BindProperty]
    public required Request LoginRequest { get; set; }

    public string? ErrorMessage { get; set; }

    public LoginModel(IConfiguration config)
    {
        _databaseUrl = config.GetValue<string>("DatabaseUrl") ?? throw new InvalidOperationException("DatabaseUrl not set in appsettings");
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl)
    {
        var connectionString = $"Server={_databaseUrl};User Id={LoginRequest.Username};Password={LoginRequest.Password};";

        using var connection = new SqlConnection(connectionString);

        ErrorMessage = await TryLogInAsync(connection);
        if (!string.IsNullOrEmpty(ErrorMessage)) return Page();
        bool isAdmin = await GetIsAdminAsync(connection);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, LoginRequest.Username),
            new Claim(ClaimTypes.Role, isAdmin ? "admin" : "user"),
        };

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
}
