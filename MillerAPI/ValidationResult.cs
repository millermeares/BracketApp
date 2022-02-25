using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillerAPI
{
    public class ValidationResult
    {
        public bool Valid { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public ValidationResult(bool valid, string message)
        {
            Message = message;
            Valid = valid;
        }
        public static ValidationResult MakeResult(bool valid, string message = ".invalid.")
        {
            return new ValidationResult(valid, message);
        }
    }
}
