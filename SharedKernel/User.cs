using SharedKernel.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel
{
    public class User 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
