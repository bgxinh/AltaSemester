using AltaSemester.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public GroupPermisson GroupPermission { get; set; }
        public string PermissionName {  get; set; }
        public virtual ICollection<RolePermission>? RolePermissions { get; set; } = new List<RolePermission>();
    }
}
