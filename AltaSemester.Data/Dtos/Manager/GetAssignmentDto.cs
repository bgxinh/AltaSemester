using AltaSemester.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.Manager
{
    public class GetAssignmentDto
    {
        public string? ServiceCode { get; set; }
        public byte? Status { get; set; }
        public string? DeviceCode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
