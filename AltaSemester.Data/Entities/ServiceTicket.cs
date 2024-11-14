using AltaSemester.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Data.Entities
{
    public class ServiceTicket
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ServiceName { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public TicketStatus Status { get; set; }
        public string DeviceName { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
