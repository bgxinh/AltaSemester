using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class DeviceDto
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DeviceUsername { get; set; }
        public string DevicePassword { get; set; }
        public string DeviceIP { get; set; }
    }
}
