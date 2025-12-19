namespace EmployeeManagmentAPI.DTOS
{
    public class UpdateUserDTO
    {
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; } // New property

    }
}
