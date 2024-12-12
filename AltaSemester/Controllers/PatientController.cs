﻿using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Patient;
using AltaSemester.Service.Cores;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AltaSemester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private ModelResult _result;
        private CountDto _count;
        private TicketStatusDto _ticketStatus;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
            _result = new ModelResult();
            _count = new CountDto();
            _ticketStatus = new TicketStatusDto();
        }
        [AllowAnonymous]
        [HttpPost("create-ticket")]
        public async Task<IActionResult> CreateTicketAsync([FromBody] TicketDto ticketDto)
        {
            _result = await _patientService.CreateTicketAsync(ticketDto);
            return Ok(_result);
        }
        [HttpGet("GetCountTicket")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetCountTicket()
        {
            _ticketStatus = await _patientService.CountTicket();
            return Ok(_ticketStatus);
        }
    }
}
