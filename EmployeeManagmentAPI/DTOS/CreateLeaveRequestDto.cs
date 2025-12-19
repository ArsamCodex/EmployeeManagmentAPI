namespace EmployeeManagmentAPI.DTOS
{
    public class CreateLeaveRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }   // Sick, Vacation, etc.
        public string Reason { get; set; }
    }
}
