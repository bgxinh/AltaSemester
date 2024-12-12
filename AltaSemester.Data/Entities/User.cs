using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public bool? IsFirstLogin { get; set; }
        public string? PasswordReset { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UserRole { get; set; }
        public string? Note { get; set; }
    }
}
