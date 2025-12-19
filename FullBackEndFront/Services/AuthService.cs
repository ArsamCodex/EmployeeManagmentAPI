using EmployeeManagmentClient.DTO;
using System.Net.Http.Json;


public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> LoginAsync(LoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/Auth2/login", dto);

        if (response.IsSuccessStatusCode)
        {
            // JWT is set in HttpOnly cookie, automatically sent with requests
            return true;
        }

        return false;
    }
}
