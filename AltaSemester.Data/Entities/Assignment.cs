using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Assignment
    {
        public string? Code { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string Telephone { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public byte Status { get; set; }
        public string ServiceCode { get; set; }
        public string DeviceCode { get; set; }
        public Service? Service { get; set; }
        public Device? Device { get; set; }
    }
}
