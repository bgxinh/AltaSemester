using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class ActivityLog
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public DateTime ActiveSessionTime { get; set; }
        public string IP {  get; set; }
        public string ActionsTaken { get; set; }
    }
}
