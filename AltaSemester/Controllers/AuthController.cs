using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.AuthDtos;
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
        public AuthController(IAuth auth )
        {
            _auth = auth;
            _result = new ModelResult();
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] RegisterDto registerDto)
        {
            _result = await _auth.Register(registerDto);
            return Ok(_result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _result = await _auth.Login(loginDto);
            return Ok(_result);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto refreshDto)
        {
            _result = await _auth.Refresh(refreshDto);
            return Ok(_result);
        }
        [HttpGet("verify/{hashedEmail}")]
        public async Task<IActionResult> Verify(string hashedEmail)
        {
            _result = await _auth.EmailConfirm(hashedEmail);
            return Ok(_result);
        }
        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] string email)
        {
            _result = await _auth.ResetPassword(email);
            return Ok(_result);
        }
    }
}
