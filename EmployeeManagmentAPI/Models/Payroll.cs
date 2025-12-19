namespace EmployeeManagmentAPI.Models
{
    public class Payroll
    {
        public int PayrollId { get; set; }
        // Primary key for this payroll record

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Links this payroll record to a specific user (employee)

        public decimal BasicSalary { get; set; }
        // Base salary of the employee

        public decimal Bonus { get; set; }
        // Any additional bonus for the employee

        public decimal Deductions { get; set; }
        // Amounts deducted (tax, penalties, etc.)

        public decimal NetSalary => BasicSalary + Bonus - Deductions;
        // Computed property: the actual amount the employee receives

        public DateTime PaymentDate { get; set; }
        // The date this payroll was paid
    }


}
/*
 * Purpose

Represents a salary record for a specific user.

Stores basic salary, bonuses, deductions, and calculates net salary.

Ties to a user via UserId and User navigation property.
var user = new ApplicationUser
{
    Id = "u1",
    FirstName = "Alice",
    LastName = "Smith"
};

var payroll = new Payroll
{
    User = user,
    UserId = user.Id,
    BasicSalary = 5000m,
    Bonus = 500m,
    Deductions = 200m,
    PaymentDate = DateTime.Now
};

// Access NetSalary
Console.WriteLine($"Employee: {payroll.User.FirstName}");
Console.WriteLine($"Basic Salary: {payroll.BasicSalary}");
Console.WriteLine($"Bonus: {payroll.Bonus}");
Console.WriteLine($"Deductions: {payroll.Deductions}");
Console.WriteLine($"Net Salary: {payroll.NetSalary}");
Console.WriteLine($"Payment Date: {payroll.PaymentDate.ToShortDateString()}");
Employee: Alice
Basic Salary: 5000
Bonus: 500
Deductions: 200
Net Salary: 5300
Payment Date: 12/15/2025
*/