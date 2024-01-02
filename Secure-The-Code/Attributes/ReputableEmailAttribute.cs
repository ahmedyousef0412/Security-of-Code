using EmailRep.NET;
using System.ComponentModel.DataAnnotations;

namespace Secure_The_Code.Attributes
{
    public class ReputableEmailAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
           var email = value as string;

            var emailRepClient = (IEmailRepClient)validationContext.GetService(typeof(IEmailRepClient))!;

            //if(IsRisky(email!,emailRepClient).GetAwaiter().GetResult())
            //    return ValidationResult.Success;

            return ValidationResult.Success;

        }

        private static async Task<bool> IsRisky(string email, IEmailRepClient emailRepClient)
        {
            var reputation = await emailRepClient.QueryEmailAsync(email);

            return 
                reputation.Details.Blacklisted ||
                reputation.Details.MaliciousActivity ||
                reputation.Details.MaliciousActivityRecent ||
                reputation.Details.Spam ||
                reputation.Details.SuspiciousTld;
        }
    }
}
