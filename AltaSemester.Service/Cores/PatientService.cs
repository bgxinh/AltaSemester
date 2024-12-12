using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Patient;
using AltaSemester.Data.Entities;
using AltaSemester.Service.Cores.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            var currentDate = DateTime.UtcNow.AddHours(7).Date;
            var ticketDate = _context.Assignments.Select(t => t.AssignmentDate).FirstOrDefault();
            var existingTicket = _context.Assignments
                .Where(t => t.ServiceCode == serviceCode)
                .OrderByDescending(t => t.Code)
                .FirstOrDefault();



            string ticketNumber = serviceCode + "0001";

            if (existingTicket != null && existingTicket.AssignmentDate.Date == currentDate)
            {
                int currentTicketNumber = int.Parse(existingTicket.Code.Substring(serviceCode.Length));
                int nextTicketNumber = currentTicketNumber + 1;
                ticketNumber = serviceCode + nextTicketNumber.ToString("D4");
            }

            return ticketNumber;
        }
            
        public async Task<ModelResult> CreateTicketAsync(TicketDto ticketDto)
        {
            ModelResult _result = new ModelResult();
            try
            {
                var serviceCode = ticketDto.ServiceCode;
                var ticketNumber = GenerateTicketNumber(serviceCode);

                Assignment newTicket = _mapper.Map<Assignment>(ticketDto);
                newTicket.Code = ticketNumber;
                newTicket.AssignmentDate = DateTime.UtcNow;
                newTicket.ExpiredDate = DateTime.UtcNow.Date.AddDays(1);
                newTicket.Status = (byte)1;

                await _context.Assignments.AddAsync(newTicket);
                await _context.SaveChangesAsync();

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
                _result.Success = false;
                _result.Message = ex.Message;
                return _result;
            }
        }
        public async Task<TicketStatusDto> CountTicket()
        {
            TicketStatusDto _count = new TicketStatusDto
            {
                Total = await _context.Assignments.CountAsync(),
                Waiting = await _context.Assignments.Where(d => d.Status == 0).CountAsync(),
                Skip = await _context.Assignments.Where(d => d.Status == 1).CountAsync(),
                Used = await _context.Assignments.Where(d => d.Status == 2).CountAsync()
            };
            return _count;
        }
    }
}
