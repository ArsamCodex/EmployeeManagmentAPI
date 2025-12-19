using EmployeeManagmentAPI.Data;
using EmployeeManagmentAPI.DTOS;
using EmployeeManagmentAPI.Interface;
using EmployeeManagmentAPI.Models;
using EmployeeManagmentAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static EmployeeManagmentAPI.Controllers.UsersController;

namespace EmployeeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]


    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IpService _ipService;
        private readonly IWebHostEnvironment _env;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            JwtService jwtService,
            IConfiguration config, IEmailService emailSender, ApplicationDbContext context, IpService ipService, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _config = config;
            _emailSender = emailSender;
            _context = context;
            _ipService = ipService;
            _env = env;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .Select(u => new AdminUserListDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    PhoneNumber = u.PhoneNumber,
                    LockoutEnabled = u.LockoutEnabled,
                    LockoutEnd = u.LockoutEnd,
                    ProfileImageUrl = u.ProfileImageUrl,
                    DateOfBirth = u.DateOfBirth
                })
                .ToListAsync();

            return Ok(users);
        }








        /*
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/{id}")]
        public async Task<IActionResult> GetAdminUserDetails(string id)
        {
            var user = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.Manager)
                .Include(u => u.Subordinates)

                .Include(u => u.Assets)
                .Include(u => u.Attendances)
                .Include(u => u.LeaveRequests)
                .Include(u => u.Notifications)
                .Include(u => u.Payrolls)
                .Include(u => u.PerformanceReviews)

                .Include(u => u.ProjectAssignments)
                    .ThenInclude(pa => pa.Project)

                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var dto = new AdminUserDetailsDTO
            {
                // --- Identity ---
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,

                // --- Personal ---
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,

                // --- Employment ---
                HireDate = user.HireDate,
                JobTitle = user.JobTitle,
                Department = user.Department == null ? null : new DepartmentDTO
                {
                    Name = user.Department.Name,
                    Description = user.Department.Description
                },

                ManagerId = user.Manager == null
                    ? null
                    : $"{user.Manager.FirstName} {user.Manager.LastName}",

                Subordinates = user.Subordinates
                    .Select(s => $"{s.FirstName} {s.LastName}")
                    .ToList(),

                Assets = user.Assets.Select(a => new AssetDTO
                {
                    Name = a.Name,
                    SerialNumber = a.SerialNumber,
                    AssignedDate = a.AssignedDate
                }).ToList(),

                Attendances = user.Attendances.Select(a => new AttendanceDTO
                {
                    Date = a.Date,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut,
                    Status = a.Status
                }).ToList(),

                LeaveRequests = user.LeaveRequests.Select(l => new LeaveRequestDTO
                {
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    LeaveType = l.LeaveType,
                    Status = l.Status,
                    Reason = l.Reason
                }).ToList(),

                Payrolls = user.Payrolls.Select(p => new PayrollDTO
                {
                    BasicSalary = p.BasicSalary,
                    Bonus = p.Bonus,
                    Deductions = p.Deductions,
                    NetSalary = p.NetSalary,
                    PaymentDate = p.PaymentDate
                }).ToList(),

                PerformanceReviews = user.PerformanceReviews.Select(r => new PerformanceReviewDTO
                {
                    ReviewDate = r.ReviewDate,
                    Score = r.Score,
                    Comments = r.Comments
                }).ToList(),

                Projects = user.ProjectAssignments.Select(pa => new ProjectDTO
                {
                    ProjectName = pa.Project.Name,
                    RoleInProject = pa.RoleInProject,
                    AssignedDate = pa.AssignedDate
                }).ToList(),

                Notifications = user.Notifications.Select(n => new NotificationDTO
                {
                    Title = n.Title,
                    Message = n.Message,
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead
                }).ToList()
            };

            return Ok(dto);
        }

        */

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/{id}")]
        public async Task<IActionResult> GetAdminUserDetails(string id)
        {
            // Projection query using AsNoTracking for better performance
            var userDto = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new AdminUserDetailsDTO
                {
                    // --- Identity ---
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProfileImageUrl = u.ProfileImageUrl,
                    ContractType = u.ContractType,

                    // --- Personal ---
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,

                    // --- Employment ---
                    HireDate = u.HireDate,
                    JobTitle = u.JobTitle,

                    Department = u.Department == null ? null : new DepartmentDTO
                    {
                        Name = u.Department.Name,
                        Description = u.Department.Description
                    },

                    ManagerId = u.Manager == null ? null : u.Manager.FirstName + " " + u.Manager.LastName,

                    Subordinates = u.Subordinates
                        .Select(s => s.FirstName + " " + s.LastName)
                        .ToList(),

                    // --- Collections ---
                    Assets = u.Assets.Select(a => new AssetDTO
                    {
                        Name = a.Name,
                        SerialNumber = a.SerialNumber,
                        AssignedDate = a.AssignedDate
                    }).ToList(),

                    Attendances = u.Attendances
                        .OrderByDescending(a => a.Date)
                        .Take(50) // load latest 50 records only
                        .Select(a => new AttendanceDTO
                        {
                            Date = a.Date,
                            CheckIn = a.CheckIn,
                            CheckOut = a.CheckOut,
                            Status = a.Status
                        }).ToList(),

                    LeaveRequests = u.LeaveRequests
                        .OrderByDescending(l => l.StartDate)
                        .Select(l => new LeaveRequestDTO
                        {
                            StartDate = l.StartDate,
                            EndDate = l.EndDate,
                            LeaveType = l.LeaveType,
                            Status = l.Status,
                            Reason = l.Reason
                        }).ToList(),

                    Payrolls = u.Payrolls
                        .OrderByDescending(p => p.PaymentDate)
                        .Take(20) // latest 20 payrolls
                        .Select(p => new PayrollDTO
                        {
                            BasicSalary = p.BasicSalary,
                            Bonus = p.Bonus,
                            Deductions = p.Deductions,
                            NetSalary = p.NetSalary,
                            PaymentDate = p.PaymentDate
                        }).ToList(),

                    PerformanceReviews = u.PerformanceReviews
                        .OrderByDescending(r => r.ReviewDate)
                        .Select(r => new PerformanceReviewDTO
                        {
                            ReviewDate = r.ReviewDate,
                            Score = r.Score,
                            Comments = r.Comments
                        }).ToList(),

                    Projects = u.ProjectAssignments
                        .Select(pa => new ProjectDTO
                        {
                            ProjectName = pa.Project.Name,
                            RoleInProject = pa.RoleInProject,
                            AssignedDate = pa.AssignedDate
                        }).ToList(),

                    Notifications = u.Notifications
                        .OrderByDescending(n => n.CreatedAt)
                        .Take(50) // latest 50 notifications
                        .Select(n => new NotificationDTO
                        {
                            Title = n.Title,
                            Message = n.Message,
                            CreatedAt = n.CreatedAt,
                            IsRead = n.IsRead
                        }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (userDto == null)
                return NotFound();

            return Ok(userDto);
        }
        [HttpGet("attendance/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserAttendance(string userId)
        {
            var attendances = await _context.Attendances
                                            .Where(a => a.UserId == userId)
                                            .OrderByDescending(a => a.Date)
                                            .Select(a => new
                                            {
                                                a.Date,
                                                a.CheckIn,
                                                a.CheckOut,
                                                a.Status
                                            })
                                            .ToListAsync();

            if (!attendances.Any())
                return NotFound();

            return Ok(attendances);
        }



        // POST: api/Users/leave-requests
        [HttpPost("leave-requests")]
        public async Task<IActionResult> CreateLeaveRequest(
            [FromBody] CreateLeaveRequestDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest("Start date cannot be after end date.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var leaveRequest = new LeaveRequest
            {
                UserId = user.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                LeaveType = dto.LeaveType,
                Reason = dto.Reason,
                Status = "Pending"   // Always pending on creation
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            Console.WriteLine("LeaveRequest request added to database");
            return Ok(new
            {
                Message = "Leave request submitted successfully",
                leaveRequest.LeaveRequestId,
                leaveRequest.Status
            });
        }
        [HttpGet("leave-requests")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            // Get current user ID from JWT
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User not found");

            var leaveRequests = await _context.LeaveRequests
                .Where(l => l.UserId == userId)
                .Select(l => new LeaveRequestDTO
                {
                    LeaveRequestId = l.LeaveRequestId,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    LeaveType = l.LeaveType,
                    Status = l.Status,
                    Reason = l.Reason
                })
                .ToListAsync();

            return Ok(leaveRequests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/leave-requests")]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var data = await _context.LeaveRequests
                .Select(l => new LeaveRequestAdminDto
                {
                    LeaveRequestId = l.LeaveRequestId,
                    UserEmail = l.User.Email,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    Reason = l.Reason
                })
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();

            return Ok(data);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/Allleave-requests/{id}/status")]
        public async Task<IActionResult> UpdateLeaveStatus(int id, [FromBody] string status)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);
            if (leave == null)
                return NotFound();

            leave.Status = status;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Leave Request",
                Description = $"Leave Request Approved By This Admin {currentUser}",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,
            });
            await _context.SaveChangesAsync();

            return Ok();
        }




        //for admin to get all leav requests
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/Allleave-requests")]
        public async Task<IActionResult> AllLeaveRequests()
        {
            var requests = await _context.LeaveRequests
                .AsNoTracking()
                .OrderBy(l => l.Status == LeaveStatuses.Pending ? 0 : 1) // pending first
                .ThenByDescending(l => l.StartDate)
                .Select(l => new LeaveRequestAdminDto
                {
                    LeaveRequestId = l.LeaveRequestId,
                    UserEmail = l.User.Email,
                    LeaveType = l.LeaveType,
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    Status = l.Status,
                    Reason = l.Reason
                })
                .ToListAsync();

            return Ok(requests);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/Allleave-requests/{id}")]
        public async Task<IActionResult> UpdateAllLeaveRequestStatus(
    int id,
    [FromBody] UpdateLeaveStatusDto dto)
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("UserId Zero");
       
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();


            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status is required");

            if (dto.Status != LeaveStatuses.Pending &&
                dto.Status != LeaveStatuses.Approved &&
                dto.Status != LeaveStatuses.Rejected)
                return BadRequest("Invalid status value");

            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(l => l.LeaveRequestId == id);

            if (leaveRequest == null)
                return NotFound("Leave request not found");

            leaveRequest.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                leaveRequest.LeaveRequestId,
                leaveRequest.Status
            });
        }

























    
        //should be enum
        public static class LeaveStatuses
        {
            public const string Pending = "Pending";
            public const string Approved = "Approved";
            public const string Rejected = "Rejected";
        }

        

       

       
       

       

   

       

        

       

       

    

       

    }
}