using System.Data.SqlClient;

namespace DatabaseAssessmentTool.Web.Services
{
    public interface IAssessmentToolDbProvider
    {
        SqlConnection Provide();
        void UpdateCredentials(string username, string password);
    }
}