using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Services.ViewModels
{
    public class LoginModel : IValidatableObject
    {
       
        public string UserName { get; set; } = null!;

       
        public string Email { get; set; } = null!;

		[DataType(DataType.Password)] 
        public string Password { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           
            //Email
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email is required.", new[] { nameof(Email) });
            }
            else
            {
                if (!IsValidEmail(Email))
                {
                    yield return new ValidationResult("Invalid Email Address", new[] { nameof(Email) });
                }
            }
            // Password
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Password is required.", new[] { nameof(Password) });
            }
            else
            {
                if (!IsValidPassword(Password))
                {
                    yield return new ValidationResult("Password should have atleast a capital , small and digit and should " +
                        "be greater than 8 and less than 20 letters.", new[] { nameof(Password) });
                }
            }
            
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsValidPassword(string password)
        {
            if (password.Length < 8 || password.Length > 20)
            {
                return false;
            }
            var hasUpperCase = new Regex(@"[A-Z]+").IsMatch(password);
            var hasLowerCase = new Regex(@"[a-z]+").IsMatch(password);
            var hasDigit = new Regex(@"[0-9]+").IsMatch(password);
            var hasSpecialChar = new Regex(@"[!@#$%^&*()_+}{:;',.?\|]").IsMatch(password);
            if (!(hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar))
            {
                return false;
            }
            var commonPatterns = new string[] { "password", "123456", "qwerty", "abc123" };
            if (commonPatterns.Contains(password.ToLower()))
            {
                return false;
            }
            return true;
        }
    }


}
