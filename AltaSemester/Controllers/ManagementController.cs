﻿using AltaSemester.Data.Dtos;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddNewUser(Registration registration)
        {
            _result = await _managementService.AddNewUser(registration);
            return Ok(_result);
        }
        [HttpPut("EditUser")]
        [Authorize(Roles ="Staff,Admin,Doctor")]
        public async Task<IActionResult> EditUser(string token, string username, EditUserDto editUserDto)
        {
            _result = await _managementService.EditUser(token, username, editUserDto);
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
        [HttpGet("GetUserPage")]
        [Authorize(Roles ="Admin,Staff,Doctor")]
        public async Task<IActionResult> GetUserPage(int pageNumber, int pageSize, string role)
        {
            _result = await _managementService.GetUserPage(pageNumber, pageSize, role);
            return Ok(_result);
        }
        [HttpGet("GetAssignmentPage")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetAssignmentPage(int pageNumber, int pageSize, GetAssignmentDto assignmentDto)
        {
            _result = await _managementService.GetAssignmentPage(pageNumber,pageSize, assignmentDto);
            return Ok(_result);
        }
        [HttpGet("DoctorGetAssignmentPage")]
        [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> DoctorGetAssignmentPage(string token, int pageNumber, int pageSize)
        {
            _result = await _managementService.DoctorGetAssignmentPage(token, pageNumber, pageSize);
            return Ok(_result);
        }
        [HttpDelete("DeleteUser")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            _result = await _managementService.DeleteUser(username);
            return Ok(_result);
        }
    }
}