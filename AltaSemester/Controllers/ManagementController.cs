using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Manager;
using AltaSemester.Data.Migrations;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

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
        public async Task<IActionResult> AddNewUser([FromBody]Registration registration)
        {
            _result = await _managementService.AddNewUser(registration);
            return Ok(_result);
        }
        [HttpPut("EditUser")]
        [Authorize(Roles ="Staff,Admin,Doctor")]
        public async Task<IActionResult> EditUser([FromBody]EditUserDto editUserDto)
        {
            _result = await _managementService.EditUser(Request.Headers["Authorization"], editUserDto);
            if(_result.Success == false)
            {
                return new ObjectResult(new { result = _result.Message })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
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
        [HttpGet("GetUserPage/{role}/{pageNumber}/{pageSize}")]
        [Authorize(Roles ="Admin,Staff,Doctor")]
        public async Task<IActionResult> GetUserPage(int pageNumber, int pageSize, string? role)
        {
            _result = await _managementService.GetUserPage(pageNumber, pageSize, role);
            return Ok(_result);
        }
        [HttpGet("GetAssignmentPage/{ServiceCode}/{From}/{To}/{DeviceCode}/{Status}/{pageNumber}/{pageSize}")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetAssignmentPage(string ServiceCode, string From, string To, string DeviceCode, string Status, int pageNumber,int pageSize)
        {
            _result = await _managementService.GetAssignmentPage( ServiceCode,  From,  To,  DeviceCode,  Status,  pageNumber,  pageSize);
            return Ok(_result);
        }
        [HttpGet("DoctorGetAssignmentPage/{pageNumber}/{pageSize}")]
        [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> DoctorGetAssignmentPage(int pageNumber, int pageSize)
        {
            _result = await _managementService.DoctorGetAssignmentPage(Request.Headers["Authorization"], pageNumber, pageSize);
            return Ok(_result);
        }
        [HttpDelete("DeleteUser")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteUser([FromBody]string username)
        {
            _result = await _managementService.DeleteUser(username);
            return Ok(_result);
        }
        [HttpPost("ImportUserExcel")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ImportUserFrpmExcel([FromForm]FileImportRequest fileImportRequest)
        {
            _result = await _managementService.AddUserFormExcel(fileImportRequest);
            return Ok(_result);
        }
        [HttpPost("ChangeAvatar")]
        [Authorize(Roles ="Admin,Staff,Doctor")]
        public async Task<IActionResult> UpdateAvatar([FromForm]FileImportRequest fileImportRequest)
        {
            _result = await _managementService.UpdateAvatar(fileImportRequest, Request.Headers["Authorization"]);
            return Ok(_result);
        }
        [HttpPut("ChangeStatusAssignment")]
        [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> ChangeStatusAssignment([FromBody]string assignmentCode)
        {
            _result = await _managementService.ChangStatusAssignment(assignmentCode);
            return Ok(_result);
        }
    }
}
