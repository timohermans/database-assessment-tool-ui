using Dapper.FluentMap;
using DatabaseAssessmentTool.Web.Models;
using System.Data.SqlClient;
using System.Security.Claims;

namespace DatabaseAssessmentTool.Web.Services;

public class AssessmentToolDbProvider : IAssessmentToolDbProvider
{
    private readonly string _databaseUrl;
    private string? _username;
    private string? _password;

    private string ConnectionString => $"Server={_databaseUrl};Database=DBExpert;User Id={_username};Password={_password};";

    public AssessmentToolDbProvider(IConfiguration config, IHttpContextAccessor httpContextAccessor, IPasswordProtector protector)
    {
        _databaseUrl = config.GetValue<string>("DatabaseUrl") ?? throw new InvalidOperationException("DatabaseUrl not set in appsettings");
        _username = httpContextAccessor.HttpContext?.User.Identity?.Name;
        var protectedPassword = httpContextAccessor.HttpContext?.User.FindFirstValue(KeyConstants.ClaimKeyDatabasePassword);
        _password = string.IsNullOrEmpty(protectedPassword) ? null : protector.Unprotect(protectedPassword);
    }

    public SqlConnection Provide(string? databaseName = null) => new SqlConnection(databaseName == null ? ConnectionString : ConnectionString.Replace("DBExpert", databaseName));

    public void UpdateCredentials(string username, string password)
    {
        _username = username;
        _password = password;
    }
}
