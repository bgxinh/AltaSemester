using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class LoginResponse
    {
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role {  get; set; }
        public string? Note { get; set; }
    }
}
