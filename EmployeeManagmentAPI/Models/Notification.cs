namespace EmployeeManagmentAPI.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        // Primary key for the notification

        public string Title { get; set; }
        // Short title of the notification (e.g., "New Leave Request")

        public string Message { get; set; }
        // Detailed message of the notification

        public DateTime CreatedAt { get; set; }
        // Timestamp when the notification was created

        public bool IsRead { get; set; }
        // Indicates whether the user has read the notification

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        // Links the notification to a specific user (the recipient)
    }


}
/* 
 * Purpose

Represents a message or alert for a user in the system.

Can be used for system alerts, reminders, or updates.

Each notification is tied to one user via UserId and User.

Tracks whether the user has read it (IsRead).

var user = new ApplicationUser
{
    Id = "u1",
    FirstName = "Alice",
    LastName = "Smith"
};

var notification = new Notification
{
    User = user,
    UserId = user.Id,
    Title = "Leave Request Approved",
    Message = "Your leave request from 20/12/2025 to 25/12/2025 has been approved.",
    CreatedAt = DateTime.Now,
    IsRead = false
};

// Access notification data
Console.WriteLine($"To: {notification.User.FirstName}");
Console.WriteLine($"Title: {notification.Title}");
Console.WriteLine($"Message: {notification.Message}");
Console.WriteLine($"Created At: {notification.CreatedAt}");
Console.WriteLine($"Is Read: {notification.IsRead}");

To: Alice
Title: Leave Request Approved
Message: Your leave request from 20/12/2025 to 25/12/2025 has been approved.
Created At: 12/15/2025 10:30:00 AM
Is Read: False




User submits a leave request → you create a LeaveRequest object and save it in the database.

System creates a notification → you create a Notification object with info like:

Title: "New Leave Request"

Message: "Alice Smith requested leave from 12/20/2025 to 12/25/2025"

Recipient: the manager (UserId = manager.Id)

Manager sees the notification in their dashboard or notification panel.
*/