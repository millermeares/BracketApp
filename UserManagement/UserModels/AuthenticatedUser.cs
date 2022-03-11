using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Roles;

namespace UserManagement.UserModels
{
    public class AuthenticatedUser : User
    {
        public UserID ID { get; set; } = new UserID();
        public List<Role> Roles { get; set; } = new List<Role>();
        public AuthenticatedUser(string username, string id) : this(username, new UserID(id)) { }

        public AuthenticatedUser(string username, UserID id)
        {
            ID = id;
            Username = username;
        }

        public void AddRole(string roleKey)
        {
            Role role = new Role() { Name = roleKey };
            Roles.Add(role);
        }

        public bool HasRole(string roleKey)
        {
            return Roles.Any(r => r.Name == roleKey);
        }

        public static AuthenticatedUser MakeEmpty()
        {
            return new AuthenticatedUser(string.Empty, UserID.MakeEmpty());
        }
        public bool IsEmpty()
        {
            return ID.IsEmpty();
        }

    }
}
