using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EmployeeManagmentClient.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly JwtAuthStateProvider _auth; // Your JWT provider
        private readonly NavigationManager _nav;

        public ApiClient(HttpClient http, JwtAuthStateProvider auth, NavigationManager nav)
        {
            _http = http;
            _auth = auth;
            _nav = nav;
        }

        private async Task<bool> SetAuthorizationHeaderAsync()
        {
            var token = await _auth.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _nav.NavigateTo("/login");
                return false;
            }

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        // CREATE
        public async Task<HttpResponseMessage> CreateAsync<T>(string url, T model)
        {
            if (!await SetAuthorizationHeaderAsync()) return null;
            return await _http.PostAsJsonAsync(url, model);
        }

   

        // READ (single entity)
        public async Task<T> GetAsync<T>(string url)
        {
            if (!await SetAuthorizationHeaderAsync()) return default;
            return await _http.GetFromJsonAsync<T>(url);
        }

        // READ (list)
        public async Task<List<T>> GetListAsync<T>(string url)
        {
            if (!await SetAuthorizationHeaderAsync()) return null;
            return await _http.GetFromJsonAsync<List<T>>(url);
        }

        // UPDATE
        public async Task<HttpResponseMessage> UpdateAsync<T>(string url, T model)
        {
            if (!await SetAuthorizationHeaderAsync()) return null;
            return await _http.PutAsJsonAsync(url, model);
        }

        // DELETE
        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            if (!await SetAuthorizationHeaderAsync()) return null;
            return await _http.DeleteAsync(url);
        }





        // POST with raw HttpContent (for files, form-data, etc.)
        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            if (!await SetAuthorizationHeaderAsync()) return null;
            return await _http.PostAsync(url, content);
        }

    }
}
