namespace EmployeeManagmentAPI.DTOS
{
    public class LeaveRequestDTO
    {
        public int LeaveRequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }
}
