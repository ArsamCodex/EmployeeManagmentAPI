namespace EmployeeManagmentAPI.DTOS
{
    public class PerformanceReviewDTO
    {
        public DateTime ReviewDate { get; set; }
        public int Score { get; set; }
        public string Comments { get; set; }
    }
}
