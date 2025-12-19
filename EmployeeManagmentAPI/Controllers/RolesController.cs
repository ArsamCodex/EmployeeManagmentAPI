using EmployeeManagmentAPI.Data;
using EmployeeManagmentAPI.Models;
using EmployeeManagmentAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeeManagmentAPI.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // only admins can call these endpoints
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IpService _ipService;

        public RolesController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IpService ipService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _ipService = ipService;
        }

        // Get all users with their roles
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var usersWithRoles = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles,
                              ur => ur.RoleId,
                              r => r.Id,
                              (ur, r) => r.Name)
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(usersWithRoles);
        }


        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] RoleChangeDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Get current logged-in user
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "AddRole",
                Description = $"Role '{model.Role}' added to user '{user.Email}'",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser
            });
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RoleChangeDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var result = await _userManager.RemoveFromRoleAsync(user, model.Role);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Get current logged-in user
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();

            // Audit log
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "RemoveRole",
                Description = $"Role '{model.Role}' removed from user '{user.Email}'",
                Timestamp = DateTime.UtcNow,
                IpAdress= IpUser
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("audit-logs")]

        public async Task<ActionResult<List<AuditLog>>> GetAuditLogs()
        {
            return await _context.AuditLogs
                                 .OrderByDescending(l => l.Timestamp)
                                 .ToListAsync();
        }

    }

    public class RoleChangeDto
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }

}
