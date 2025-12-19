using static System.Net.WebRequestMethods;

namespace EmployeeManagmentAPI.Services
{
    public class IpService
    {
      
            private readonly HttpClient _httpClient;

            public IpService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<string> GetPublicIpAsync()
            {
                try
                {
                    return await _httpClient.GetStringAsync("https://api.ipify.org");
                }
                catch
                {
                    return "Unable to retrieve IP";
                }
            }
       
        }

    }


