using AltaSemester.Data.Abstractions.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class User : IAccountStatus
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<ServiceTicket>? ServiceTickets { get; set; } = new List<ServiceTicket>();
    }
}
