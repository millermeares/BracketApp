using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.UserModels
{
    internal class AuthenticatedUser : User
    {
        public string UserID { get; set; } = string.Empty;
    }
}
