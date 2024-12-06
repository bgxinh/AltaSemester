using AltaSemester.Data.Dtos;
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
        public DeviceController(IDevice device)
        {
            _device = device;
            _result = new ModelResult();
        }
        [HttpGet("GetDevice")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetDevice()
        {
            _result = await _device.GetAllDevice();
            return Ok(_result);
        }
        [HttpGet("GetDevicePage")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetDevicePage(int pageNumber, int pageSize, bool? Status, bool? StatusConnect)
        {
            _result = await _device.GetDevicePage(pageNumber, pageSize, Status, StatusConnect);
            return Ok(_result);
        }
        [HttpPut("EditDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> EditDevice(string deviceCode, DeviceDto deviceDto)
        {
            _result = await _device.EditDevice(deviceCode, deviceDto);
            return Ok(_result);
        }
        [HttpDelete("DeleteDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteDevice(string deviceCode)
        {
            _result = await _device.DeleteDevice(deviceCode);
            return Ok(_result);
        }
        [HttpPost("AddNewDevice")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AddNewDevice(DeviceDto deviceDto)
        {
            _result = await _device.AddNewDevice(deviceDto);
            return Ok(_result);
        }
        [HttpPost("ImportDeviceFromExcel")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ImportDeviceFromExcel(FileImportRequest fileImportRequest)
        {
            _result = await _device.ImportDeviceFromExcel(fileImportRequest);
            return Ok(_result);
        }
    }
}
