using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement
{
    public class AuthToken
    {
        public string Token { get; set; } = string.Empty;
        public AuthToken(string token)
        {
            Token = token;
        }
        public static AuthToken Make()
        {
            return new AuthToken(Guid.NewGuid().ToString());
        }
    }
}
