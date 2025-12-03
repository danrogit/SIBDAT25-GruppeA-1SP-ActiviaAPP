using ActiviaAPP.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiviaAPP.Classes
{
    public class User
    {
        // string.Empty = Tester om string er null, eller om der er noget i den, så vi kan bruge den
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{FullName} ({Username})";
        }

    }
}
