using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.Auth;
using AltaSemester.Data.Dtos.Device;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Patient;
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
    public class DeviceController : ControllerBase
    {
        private readonly IDevice _device;
        private ModelResult _result;
        private CountDto _count;
        public DeviceController(IDevice device)
        {
            _device = device;
            _result = new ModelResult();
            _count = new CountDto();
        }
        [HttpGet("GetDevice")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetDevice()
        {
            _result = await _device.GetAllDevice();
            return Ok(_result);
        }
        [HttpGet("GetDevicePage/{pageNumber}/{pageSize}/{Status}/{StatusConnect}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetDevicePage(int pageNumber, int pageSize, string? Status, string? StatusConnect)
        {
            _result = await _device.GetDevicePage(pageNumber, pageSize, Status, StatusConnect);
            return Ok(_result);
        }
        [HttpPut("EditDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> EditDevice([FromBody] DeviceDto deviceDto)
        {
            _result = await _device.EditDevice(deviceDto);
            return Ok(_result);
        }
        [HttpDelete("DeleteDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteDevice([FromBody]string deviceCode)
        {
            _result = await _device.DeleteDevice(deviceCode);
            return Ok(_result);
        }
        [HttpPost("AddNewDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AddNewDevice([FromBody]DeviceDto deviceDto)
        {
            _result = await _device.AddNewDevice(deviceDto);
            return Ok(_result);
        }
        [HttpPost("ImportDeviceFromExcel")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ImportDeviceFromExcel([FromForm]FileImportRequest fileImportRequest)
        {
            _result = await _device.ImportDeviceFromExcel(fileImportRequest);
            return Ok(_result);
        }
        [HttpGet("GetCountDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CountDevices()
        {
            _count = await _device.CountDevices();
            return Ok(_count);
        }
        [HttpPost("LoginDevice")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginDevice([FromBody]LoginDto loginDto)
        {
            _result = await _device.LoginDevice(loginDto.Username, loginDto.Password);
            return Ok(_result);
        }
        [HttpPost("LogoutDevice")]
        [AllowAnonymous]
        public async Task<IActionResult> LogoutDevice(string username)
        {
            _result = await _device.LogoutDevice(username);
            return Ok(_result);
        }
    }
}
