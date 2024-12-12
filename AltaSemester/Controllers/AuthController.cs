﻿using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AltaSemester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _auth;
        private ModelResult _result;
        public AuthController(IAuth auth)
        {
            _auth = auth;
            _result = new ModelResult();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _result = await _auth.Login( loginDto.Username, loginDto.Password);
            return Ok(_result);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh ([FromBody] RefreshDto refresh)
        {
            _result = await _auth.Refresh(refresh.AccessToken, refresh.RefreshToken);
            return Ok(_result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout (string Username)
        {
            _result = await _auth.Logout(Username);
            return Ok(_result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPassword resetPassword)
        {
            _result = await _auth.ResetPassword(resetPassword);
            return Ok(_result);
        }
        [HttpPost("ResetPasswordFirstLogin")]
        public async Task<IActionResult> ResetPasswordFirstLogin([FromBody]ResetPassword resetPassword)
        {
            _result = await _auth.ResetPasswordFirstLogin(resetPassword);
            return Ok(_result);
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody]string email)
        {
            _result = await _auth.ForgotPassword(email);
            return Ok(_result);
        }
    }
}
