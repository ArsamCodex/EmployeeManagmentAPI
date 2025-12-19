namespace EmployeeManagmentAPI.Models
{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        // Primary key for the leave request

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Links the leave request to the user who requested it

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // The period for which the leave is requested

        public string LeaveType { get; set; }
        // Type of leave, e.g., "Sick", "Vacation", "Personal"

        public string Status { get; set; }
        // Current status of the leave, e.g., "Pending", "Approved", "Rejected"

        public string Reason { get; set; }
        // Reason for requesting the leave
    }


}

/*Purpose

Represents a leave request made by a user.

Tracks:

Who requested it (UserId / User)

When the leave starts and ends (StartDate / EndDate)

The type of leave (LeaveType)

Approval status (Status)

Reason for leave (Reason)

Can be used in HR management systems to approve or reject leaves.


var user = new ApplicationUser
{
    Id = "u1",
    FirstName = "Alice",
    LastName = "Smith"
};

var leaveRequest = new LeaveRequest
{
    User = user,
    UserId = user.Id,
    StartDate = new DateTime(2025, 12, 20),
    EndDate = new DateTime(2025, 12, 25),
    LeaveType = "Vacation",
    Status = "Pending",
    Reason = "Family trip"
};

// Access leave request data
Console.WriteLine($"User: {leaveRequest.User.FirstName}");
Console.WriteLine($"Leave Type: {leaveRequest.LeaveType}");
Console.WriteLine($"From: {leaveRequest.StartDate.ToShortDateString()} To: {leaveRequest.EndDate.ToShortDateString()}");
Console.WriteLine($"Reason: {leaveRequest.Reason}");
Console.WriteLine($"Status: {leaveRequest.Status}");
User: Alice
Leave Type: Vacation
From: 12/20/2025 To: 12/25/2025
Reason: Family trip
Status: Pending
*/
