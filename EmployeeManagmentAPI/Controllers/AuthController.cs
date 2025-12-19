using EmployeeManagmentAPI.Data;
using EmployeeManagmentAPI.DTOS;
using EmployeeManagmentAPI.Interface;
using EmployeeManagmentAPI.Models;
using EmployeeManagmentAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IpService _ipService;
        private readonly IWebHostEnvironment _env;
        public AuthController(
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Generate a confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode token to make it URL-safe
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Build confirmation URL to your frontend
            string confirmUrl =
                $"{_config["Frontend:BaseUrl"]}/confirm-email?userId={user.Id}&token={encodedToken}";
            Console.WriteLine(confirmUrl);
            /*
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm your account",
                $"Please confirm your account by clicking this link: <a href='{confirmUrl}'>Confirm Email</a>"
            );*/
            // Audit log
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Registered User",
                Description = $"This User Joined Tghe Platform",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,
            });
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Registration successful. Check your email to confirm your account." });
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return BadRequest("Invalid user.");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return BadRequest("Invalid or expired token.");
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Confirmed Email",
                Description = $"This User Confirmed Email",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Email confirmed successfully." });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
                return Unauthorized("Invalid credentials");

            // <-- NEW: Check if user has 2FA enabled
            if (user.TwoFactorEnabled)
            {
                // Don't issue JWT yet, return requires2FA response
                return Ok(new
                {
                    requires2FA = true,
                    userId = user.Id
                });
            }

            // No 2FA, issue JWT as usual
            var token = _jwtService.GenerateToken(user);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
                var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = user.Email,
                Action = "Login Action",
                Description = $"This User loged in Tghe Platform",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,
               // AdminInCase = currentUserId

            });
            await _context.SaveChangesAsync();
            return Ok(new
            {
                token,
                email = user.Email,
                userId = user.Id,
                expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["JwtSettings:ExpiresInMinutes"]))
            });
        }

        [HttpPost("login-2fa")]
        public async Task<IActionResult> Login2FA([FromBody] Login2FaDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Unauthorized("Invalid user");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code);

            if (!isValid)
                return Unauthorized("Invalid 2FA code");

            // 2FA verified → issue JWT
            var token = _jwtService.GenerateToken(user);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Logged in 21FA User",
                Description = $"This User Logged in Tghe Platform",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new
            {
                token,
                email = user.Email,
                userId = user.Id,
                expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["JwtSettings:ExpiresInMinutes"]))
            });
        }


        [HttpPost("enable-2fa")]
        public async Task<IActionResult> Enable2FA([FromBody] Enable2FaDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("User not found");

            // Generate a key if one doesn't exist
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Create QR code URI for Google Authenticator
            string email = user.Email;
            string issuer = _config["AppSettings:Issuer"] ?? "MyApp";
            string qrCodeUri = $"otpauth://totp/{issuer}:{email}?secret={key}&issuer={issuer}&digits=6";
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Enabled  2FA ",
                Description = $"2Fa Has Been Enabled",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new
            {
                key,
                qrCodeUri
            });
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("User not found");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                model.Code
            );

            if (!isValid) return BadRequest("Invalid 2FA code");

            // Enable 2FA for the user
            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Verify  2FA ",
                Description = $"2Fa Has Been Veryfied",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok("2FA enabled successfully");
        }

        [HttpGet("check-2fa/{userId}")]
        public async Task<IActionResult> Check2FA(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(user.TwoFactorEnabled);
        }

        [HttpPost("reset-2fa")]
        [Authorize] // Only logged-in users
        public async Task<IActionResult> Reset2FA([FromBody] Reset2FADto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("User not found");

            // Disable 2FA temporarily
            user.TwoFactorEnabled = false;
            await _userManager.UpdateAsync(user);

            // Remove old authenticator key
            await _userManager.ResetAuthenticatorKeyAsync(user);

            // Generate new key for Google Authenticator
            var key = await _userManager.GetAuthenticatorKeyAsync(user);

            var email = user.Email;
            var qrCodeUri = $"otpauth://totp/MyApp:{email}?secret={key}&issuer=MyApp&digits=6";
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Reset  2FA ",
                Description = $"2Fa Has Been Reset",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Key = key,
                QrCodeUri = qrCodeUri
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get current user ID from JWT claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized("User not found.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Change Password",
                Description = $"Password Has Been Enabled",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "Password changed successfully." });
        }


        [HttpPost("forgot-password")]
      
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("No user");
            var token  = await _userManager.GeneratePasswordResetTokenAsync(user);

            var frontEndUrl = _config["Frontend:BaseUrl"].TrimEnd('/');
            var resetLin = $"{frontEndUrl}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            Console.WriteLine(resetLin);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Forgot Password ",
                Description = $"Forgot Password Requested",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "reset link has been sent." });
        }

        [HttpPost("reset-password")]

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("No user");

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Reset Password ",
                Description = $"Reset Password Requested",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "password reset done " });
        }
        [HttpPost("disable-2fa")]
        [Authorize] // Only logged-in users
        public async Task<IActionResult> Disable2FA([FromBody] Disable2FADto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("User not found");

            // Disable 2FA
            user.TwoFactorEnabled = false;
            await _userManager.UpdateAsync(user);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();
            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = currentUser?.Email ?? "Unknown",
                Action = "Disabled 2Fa",
                Description = $"Disable 2FA Requested",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "Two-Factor Authentication has been disabled." });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Get logged-in user ID
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return BadRequest("No User Found");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User Not Found");

            // Return all relevant properties including new data members
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount,

                // Custom properties
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.ProfileImageUrl,
                user.HireDate,
                user.JobTitle,
                user.DepartmentId,
                user.ManagerId,
                user.ContractType
            });
        }
        [Authorize]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Ensure 'profiles' folder exists in wwwroot
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profiles");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Save file as userId.jpg
            var fileName = $"{userId}.jpg";
            var filePath = Path.Combine(uploadsPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(500, "The app does not have permission to write to the profiles folder.");
            }

            // Build a full URL for the file
            var url = $"{Request.Scheme}://{Request.Host}/profiles/{fileName}";

            // Update user's profile image URL in database
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.ProfileImageUrl = url;
                await _userManager.UpdateAsync(user);
                var IpUser = await _ipService.GetPublicIpAsync();
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var currentUser = await _userManager.FindByIdAsync(currentUserId);

                _context.AuditLogs.Add(new AuditLog
                {
                    UserEmail = currentUser?.Email ?? "Unknown",
                    Action = "Profile Imagew Added",
                    Description = $"This User Changed Profile Image",
                    Timestamp = DateTime.UtcNow,
                    IpAdress = IpUser,
                });
                await _context.SaveChangesAsync();

            }

            return Ok(url);
        }
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDTO model)
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("UserId Zero");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Update properties
            if (!string.IsNullOrEmpty(model.UserName))
                user.UserName = model.UserName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (model.DateOfBirth.HasValue)
                user.DateOfBirth = model.DateOfBirth.Value;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();

            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = user.Email,
                Action = "Update Profile",
                Description = $"Profile Has Been Updated",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.UserName,
                user.PhoneNumber,
                user.DateOfBirth
            });
        }

        // 1️⃣ Initiate Google OAuth
        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string redirectUri)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("ExternalLoginCallback", new { redirectUri })
            };
            return Challenge(properties, provider);
        }

        // 2️⃣ Handle callback from Google
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string redirectUri)
        {
            //Give me the identity information that the external provider (Google) just sent."It returns an AuthenticateResult,
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
                return BadRequest("External login failed.");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                return BadRequest("Email not found from provider.");

            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Create user
                user = new ApplicationUser { UserName = email, Email = email };
                await _userManager.CreateAsync(user);
            }

            // Optional 2FA
            if (user.TwoFactorEnabled)
            {
                return Ok(new
                {
                    requires2FA = true,
                    userId = user.Id
                });
            }

            // Issue JWT
            var token = _jwtService.GenerateToken(user);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var IpUser = await _ipService.GetPublicIpAsync();

            _context.AuditLogs.Add(new AuditLog
            {
                UserEmail = user.Email,
                Action = "Log In Whit Google ",
                Description = $"Logged In Whit Google",
                Timestamp = DateTime.UtcNow,
                IpAdress = IpUser,

            });
            await _context.SaveChangesAsync();
            // Redirect to frontend with token
            return Redirect($"{redirectUri}?token={token}");
        }





























        //test
        [HttpGet("whoami")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult WhoAmI()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}
