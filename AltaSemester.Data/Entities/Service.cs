using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; } = null;
        public string? Prefix { get; set; } = string.Empty;
        public string? Surfix { get; set; } = string.Empty ;
        public virtual ICollection<DeviceService> DeviceServices { get; set; } = new List<DeviceService>();
    }
}
