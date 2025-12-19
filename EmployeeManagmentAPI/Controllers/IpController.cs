using EmployeeManagmentAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace EmployeeManagmentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IpController : ControllerBase
    {
        private readonly IpService _ipService;

        public IpController(IpService ipService)
        {
            _ipService = ipService;
        }

        [HttpGet("getuserip")]
        public async Task<IActionResult> GetIp()

        {
            var ip =  await _ipService.GetPublicIpAsync();
            
            return Ok(new { ipAddress = ip });
        }

        


    }
}