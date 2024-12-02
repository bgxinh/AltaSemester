using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class Service
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; } = null;
        public bool? Status { get; set; }
        public virtual ICollection<Assignment>? Assignments { get; set; }
    }
}
