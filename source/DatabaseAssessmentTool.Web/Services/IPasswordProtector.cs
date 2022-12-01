namespace DatabaseAssessmentTool.Web.Services
{
    public interface IPasswordProtector
    {
        string Protect(string value);
        string Unprotect(string protectedValue);
    }
}