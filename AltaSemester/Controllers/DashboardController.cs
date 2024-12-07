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
        [HttpGet("{month}")]
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult GetStatisticsByMonth(int month)
        {
            var statistics = _result.GetStatisticByMonth(month);

            return Ok(statistics);
        }
    }
}
