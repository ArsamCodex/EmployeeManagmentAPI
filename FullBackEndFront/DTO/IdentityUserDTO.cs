namespace EmployeeManagmentClient.DTO
{
    public class IdentityUserDTO
    {
        // IdentityUser core properties
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        // Custom properties from ApplicationUser
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? HireDate { get; set; }
        public string? JobTitle { get; set; }

        public int? DepartmentId { get; set; }
        public string? ManagerId { get; set; }
    }

}
