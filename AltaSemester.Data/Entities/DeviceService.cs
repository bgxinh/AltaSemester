using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class DeviceService
    {
        public int ServiceId { get; set; }
        public int DeviceId {  get; set; }
        public Device? Device { get; set; }
        public Service? Service { get; set; }
    }
}
