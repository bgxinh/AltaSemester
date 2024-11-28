using AltaSemester.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.AuthDtos
{
    public class LoginResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string? Token { get; set; }
    }
}
