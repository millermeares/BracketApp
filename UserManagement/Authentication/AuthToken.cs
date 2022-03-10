using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Roles;

namespace UserManagement
{
    public class AuthToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public List<Role> Roles { get; set; } = new List<Role>();
        public AuthToken(string token, DateTime createTime) : this(token)
        {
            CreateTime = createTime;
        }
        public AuthToken(string token)
        {
            Token = token;
        }
        public AuthToken()
        {

        }
        public static AuthToken Make()
        {
            return new AuthToken(Guid.NewGuid().ToString());
        }

        public void AddRole(Role role)
        {
            Roles.Add(role);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Token);
        }
        public static AuthToken MakeEmpty()
        {
            return new AuthToken() { Token = string.Empty };
        }
    }
}
