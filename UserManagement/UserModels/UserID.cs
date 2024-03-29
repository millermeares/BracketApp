﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.UserModels
{
    public class UserID
    {
        public string ID { get; set; } = string.Empty;
        public UserID()
        {
            ID = Guid.NewGuid().ToString();
        }
        public UserID(string id)
        {
            ID = id;
        }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ID);
        }

        public static UserID MakeEmpty()
        {
            return new UserID(string.Empty);
        }
    }
}
