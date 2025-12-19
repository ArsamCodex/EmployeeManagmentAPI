namespace EmployeeManagmentAPI.DTOS
{
    public class Login2FaDto
    {
        public string UserId { get; set; }
        public string Code { get; set; } // 6-digit code from Google Authenticator
    }
}
