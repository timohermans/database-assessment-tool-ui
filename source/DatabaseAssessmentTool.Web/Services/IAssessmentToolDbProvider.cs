using System.Data.SqlClient;

namespace DatabaseAssessmentTool.Web.Services
{
    public interface IAssessmentToolDbProvider
    {
        SqlConnection Provide(string? databaseName = null);
        void UpdateCredentials(string username, string password);
    }
}