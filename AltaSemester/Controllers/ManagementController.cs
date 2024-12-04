using AltaSemester.Data.Dtos;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AltaSemester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementService _managementService;
        private ModelResult _result;
        public ManagementController(IManagementService managementService)
        {
            _managementService = managementService;
            _result = new ModelResult();
        }
        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNewUser(Registration registration)
        {
            _result = await _managementService.AddNewUser(registration);
            return Ok(_result);
        }
        [HttpGet("GetAssignment")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAssignment()
        {
            _result = await _managementService.GetAssignment();
            return Ok(_result);
        }
        [HttpGet("DoctorGetAssignment")]
        [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> DoctorGetAssignment()
        {
            _result = await _managementService.DoctorGetAssignment(Request.Headers["Authorization"]);
            return Ok(_result);
        }
        [HttpGet("GetAllUser")]
        [Authorize(Roles = "Admin,Staff,Doctor")]
        public async Task<IActionResult> GetAllUser()
        {
            _result = await _managementService.GetAllUsers();
            return Ok(_result);
        }
    }
}
