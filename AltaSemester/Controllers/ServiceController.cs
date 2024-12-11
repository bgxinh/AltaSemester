using AltaSemester.Data.Dtos;
using AltaSemester.Data.Dtos.File;
using AltaSemester.Data.Dtos.Patient;
using AltaSemester.Data.Dtos.Service;
using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;

namespace AltaSemester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ServiceController : ControllerBase
    {
        private readonly IService _service;
        private ModelResult _result;
        private CountDto _count;
        public ServiceController(IService service)
        {
            _service = service;
            _result = new ModelResult();
            _count = new CountDto();
        }
        [HttpGet("GetService/{Status}")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> GetService(bool? Status)
        {
            _result = await _service.GetService(Status);
            return Ok(_result);
        }
        [HttpPost("AddNewService")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> AddNewService([FromBody]ServiceDto serviceDto)
        {
            _result = await _service.AddNewService(serviceDto);
            return Ok(_result);
        }
        [HttpPut("EditService")]
        [Authorize(Roles ="Admin,Staff")]
        public async Task<IActionResult> EditService([FromBody]ServiceDto serviceDto)
        {
            _result = await _service.EditService(serviceDto);
            return Ok(_result);
        }
        [HttpDelete("DeleteService")]
        [Authorize(Roles ="Staff,Admin")]
        public async Task<IActionResult> DeleteService([FromBody]string serviceCode)
        {
            _result = await _service.DeleteService(serviceCode);
            return Ok(_result);
        }
        [HttpGet("GetCountService")]
        public async Task<IActionResult> GetCountService()
        {
            _count = await _service.CountService();
            return Ok(_count);
        }
    }
}
