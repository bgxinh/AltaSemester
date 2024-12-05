using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class ServiceDto
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; } = null;
    }
}
