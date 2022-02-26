using MillerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Authentication;
namespace UserManagement
{
    public class SigningUpUser : User
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public ValidationResult Validate()
        {
            return ValidationResult.MakeResult(Password == ConfirmPassword);
        }

        internal Password PasswordToInsert()
        {
            return Authentication.Password.Make(Password);
        }
    }
}
