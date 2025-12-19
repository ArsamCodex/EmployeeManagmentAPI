namespace EmployeeManagmentAPI.Models
{
    public class AuditLog
    {
        public int AuditLogID { get; set; }
        public string UserEmail { get; set; } // or UserId
        public string Action { get; set; }    // e.g., "LoginSuccess", "AddRole", "DeletePost"
        public string Description { get; set; } // optional extra info
        public DateTime Timestamp { get; set; }
        public string IpAdress { get; set; }
    
    }
}
