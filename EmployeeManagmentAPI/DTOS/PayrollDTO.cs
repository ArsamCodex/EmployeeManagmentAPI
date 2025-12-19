namespace EmployeeManagmentAPI.DTOS
{
    public class PayrollDTO
    {
        public decimal BasicSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
