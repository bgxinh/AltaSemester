using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Device
    {
        public string Id { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceUsername { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceIP { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<DeviceService> DeviceServices { get; set; } = new List<DeviceService>();
    }
}
