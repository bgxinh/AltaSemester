using AltaSemester.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos
{
    public class TicketDto
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string DeviceCode { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string Telephone { get; set; }
        
    }
}
