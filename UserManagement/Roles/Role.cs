using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Roles
{
    public class Role
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name.ToLower();
            set => _name = value;
        }
    }
}
