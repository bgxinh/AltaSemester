using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.Manager
{
    public class UserDto
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? UserRole { get; set; }
        public string? Note { get; set; }
        public bool? IsActive { get; set; }
    }
}
