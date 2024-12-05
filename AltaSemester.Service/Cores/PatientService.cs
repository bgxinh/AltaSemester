using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltaSemester.Service.Cores
{
    public class PatientService : IPatientService
    {
        private AltaSemesterContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public PatientService(AltaSemesterContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        private string GenerateTicketNumber(string serviceCode)
        {
            var existingTicket = _context.Assignments
                .Where(t => t.ServiceCode == serviceCode)
                .OrderByDescending(t => t.Code)
                .FirstOrDefault();

            string ticketNumber = serviceCode + "0001";

            if (existingTicket != null)
            {
                int currentTicketNumber = int.Parse(existingTicket.Code.Substring(3));
                int nextTicketNumber = currentTicketNumber + 1;
                ticketNumber = serviceCode + nextTicketNumber.ToString("D4");
            }

            return ticketNumber;
        }
            
        public async Task<ModelResult> CreateTicketAsync(TicketDto ticketDto)
        {
            ModelResult _result = new ModelResult();
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var serviceCode = ticketDto.ServiceCode;
                    var ticketNumber = GenerateTicketNumber(serviceCode);

                    Assignment newTicket = _mapper.Map<Assignment>(ticketDto);
                    newTicket.Code = ticketNumber;
                    newTicket.DeviceCode = "K01";

                    await _context.Assignments.AddAsync(newTicket);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var ticketResponse = new TicketResponse
                    {
                        Code = newTicket.Code,
                        ServiceName = ticketDto.ServiceName,
                        CustomerName = newTicket.CustomerName,
                        AssignmentDate = newTicket.AssignmentDate,
                        ExpiredDate = newTicket.ExpiredDate
                    };
                    _result.Success = true;
                    _result.Data = ticketResponse; 
                    return _result;
                }
                catch (Exception ex)
                {
                    //transaction.Rollback();
                    _result.Success = false;
                    _result.Message = ex.Message;
                    return _result;
                }
            }
        }
    }
}
