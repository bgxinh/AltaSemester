using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Device
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceUsername { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceIP { get; set; }
        public bool? IsActive { get; set; }
        public bool? Status { get; set; }
        public bool? StatusConnect {  get; set; }
        public DateTime? CreateAt { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; } 
    }
}
