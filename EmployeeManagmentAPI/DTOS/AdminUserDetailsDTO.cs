using static EmployeeManagmentAPI.Controllers.UsersController;

namespace EmployeeManagmentAPI.DTOS
{
    public class AdminUserDetailsDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }

        // Personal
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Employment
        public DateTime? HireDate { get; set; }
        public string? JobTitle { get; set; }
        public DepartmentDTO? Department { get; set; }
        public string? ManagerId { get; set; }
        public string? ContractType { get; set; }

        public List<string> Subordinates { get; set; } = new();

        public List<AssetDTO> Assets { get; set; } = new();
        public List<AttendanceDTO> Attendances { get; set; } = new();
        public List<LeaveRequestDTO> LeaveRequests { get; set; } = new();
        public List<PayrollDTO> Payrolls { get; set; } = new();
        public List<PerformanceReviewDTO> PerformanceReviews { get; set; } = new();
        public List<ProjectDTO> Projects { get; set; } = new();
        public List<NotificationDTO> Notifications { get; set; } = new();
    }
}
