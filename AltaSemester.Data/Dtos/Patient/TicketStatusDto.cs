using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Dtos.Patient
{
    public class TicketStatusDto
    {
        public int Total { get; set; }
        public int Used { get; set; }
        public int Waiting  { get; set; }
        public int Skip {  get; set; }
    }
}
