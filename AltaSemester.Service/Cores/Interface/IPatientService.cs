using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores.Interface
{
    public interface IPatientService
    {
        Task<ModelResult> CreateTicketAsync(TicketDto ticketCreateDto);
    }
}
