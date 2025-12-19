using Microsoft.AspNetCore.Identity;

namespace EmployeeManagmentAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // --- Personal Info ---
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContractType { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // --- Employee Image ---
        public string? ProfileImageUrl { get; set; } // URL or relative path to employee's photo

        // --- Employment Info ---
        public DateTime? HireDate { get; set; }
        public string? JobTitle { get; set; }

        // --- Department ---
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // --- Manager (Self-reference) ---
        public string? ManagerId { get; set; } // Id of the manager (another ApplicationUser)
        public ApplicationUser? Manager { get; set; }
        public ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();

        // --- Navigation Properties ---
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
        public ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
        public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
