using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public class LoggingInUser : User
    {
        public string Password { get; set; } = string.Empty;
    }
}
