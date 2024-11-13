using AltaSemester.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public Roles RoleName { get; set; }
        public string RoleDesciption { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
