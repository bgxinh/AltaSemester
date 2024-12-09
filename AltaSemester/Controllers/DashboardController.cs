using AltaSemester.Service.Cores.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AltaSemester.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private IDashboard _result;
        public DashboardController(IDashboard result)
        {
            _result = result;
        }
        [HttpGet("GetStatisticsByMonth/{month}")]
        [Authorize(Roles = "Admin")]
        //public IActionResult GetStatisticsByMonth(int month)
        //{
        //    var statistics = _result.GetStatisticByMonth(month);

        //    return Ok(statistics);
        //}
        public async Task<IActionResult> GetStatisticsByMonth(int month)
        {
            var statistics = await _result.GetStatisticByMonth(month);

            return Ok(statistics);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetStatisticsByWeek/{month}")]
        public async Task<IActionResult> GetStatisticsByWeek(int month)
        {
            var statistics = await _result.GetStatisticByWeek(month);

            return Ok(statistics);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetStatisticsByYear")]
        public async Task<IActionResult> GetStatisticsByYear()
        {
            var statistics = await _result.GetStatisticByYear();

            return Ok(statistics);
        }
    }
}
