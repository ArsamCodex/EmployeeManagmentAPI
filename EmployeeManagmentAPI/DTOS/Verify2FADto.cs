namespace EmployeeManagmentAPI.DTOS
{
    public class Verify2FADto
    {
        public string UserId { get; set; }
        public string Code { get; set; } // 6-digit code from Authenticator app
    }

}
