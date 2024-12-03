using AltaSemester.Data.DataAccess;
using AltaSemester.Data.Dtos;
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
        public async Task<IActionResult> Login (string username, string password)
        {
            _result = await _auth.Login(username, password);
            return Ok(_result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Registraion (Registration registration)
        {
            _result = await _auth.Registration(registration);
            return Ok(_result);
        }
    }
}
