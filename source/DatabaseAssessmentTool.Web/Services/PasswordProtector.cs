using Microsoft.AspNetCore.DataProtection;

namespace DatabaseAssessmentTool.Web.Services
{
    /*
     * solution based on:
     * https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
     */
    public class PasswordProtector : IPasswordProtector
    {
        private readonly IDataProtector _protector;

        public PasswordProtector(IDataProtectionProvider protectProvider, IConfiguration config)
        {
            var secret = config.GetValue<string?>("Secret") ?? throw new InvalidOperationException("Secret not set in appsettings"); ;
            _protector = protectProvider.CreateProtector(secret);
        }

        public string Protect(string value)
        {
            return _protector.Protect(value);
        }

        public string Unprotect(string protectedValue)
        {
            return _protector.Unprotect(protectedValue);
        }
    }
}
