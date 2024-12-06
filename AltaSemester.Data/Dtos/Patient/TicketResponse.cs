using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.Patient
{
    public class TicketResponse
    {
        public string Code { get; set; }
        public string ServiceName { get; set; }
        public string CustomerName { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
