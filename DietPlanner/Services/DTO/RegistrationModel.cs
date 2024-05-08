using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class RegistrationModel : IValidatableObject
    {

        public string UserName { get; set; } = null!;


        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }


        public string Email { get; set; } = null!;


        public string? ContactNumber { get; set; }

		[DataType(DataType.Password)]

		public string Password { get; set; } = null!;

		[DataType(DataType.Password)]

		public string ConfirmPassword { get; set; } = null!;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Username
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult("Username is required.", new[] { nameof(UserName) });
            }
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

            //Firsname
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrWhiteSpace(FirstName))
            {
                yield return new ValidationResult("First Name is required.", new[] { nameof(FirstName) });
            }
            //LastName
            if (string.IsNullOrEmpty(LastName) || string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult("Last Name  is required.", new[] { nameof(LastName) });
            }
            //PhoneNumber
            if (string.IsNullOrEmpty(ContactNumber) || string.IsNullOrWhiteSpace(ContactNumber))
            {
                yield return new ValidationResult("Phone Number is required.", new[] { nameof(ContactNumber) });
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
