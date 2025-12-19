namespace EmployeeManagmentAPI.DTOS
{
    public class NotificationDTO
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
