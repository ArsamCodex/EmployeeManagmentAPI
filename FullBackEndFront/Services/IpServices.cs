using System.Net.Http;

namespace EmployeeManagmentClient.Services
{
    public class IpServices
    {
        private readonly HttpClient _http;

        public IpServices(HttpClient http)
        {
            _http = http;
        }

    

        public async Task<string> GetPublicIpAsync()
        {
            try
            {
                return await _http.GetStringAsync("https://api.ipify.org");
            }
            catch
            {
                return "Unable to retrieve IP";
            }
        }
    }
}
