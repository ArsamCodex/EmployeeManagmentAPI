namespace EmployeeManagmentAPI.DTOS
{
    public class AttendanceDTO
    {
        public DateTime Date { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string Status { get; set; }
    }
}
