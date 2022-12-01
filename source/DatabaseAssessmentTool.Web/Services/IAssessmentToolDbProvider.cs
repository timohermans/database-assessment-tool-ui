using System.Data.SqlClient;

namespace DatabaseAssessmentTool.Web.Services
{
    public interface IAssessmentToolDbProvider
    {
        SqlConnection Connection { get; }

        void UpdateCredentials(string username, string password);
    }
}