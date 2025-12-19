using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Timers;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _anonymous =
        new ClaimsPrincipal(new ClaimsIdentity());

    private System.Threading.CancellationTokenSource? _expirationCts;
    private System.Timers.Timer? _timer;
    public string? TimeRemaining { get; private set; }

    public JwtAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;

    }


  

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_anonymous);

        StartTokenExpirationTimer(token);
        return new AuthenticationState(CreateClaimsPrincipal(token));
    }

    public async Task SetTokenAsync(string token)
    {
        await _localStorage.SetItemAsync("authToken", token);
        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(CreateClaimsPrincipal(token))));
        StartTokenExpirationTimer(token);
    }

    public async Task LogoutAsync()
    {
        _expirationCts?.Cancel();
        _timer?.Stop();
        _timer?.Dispose();

        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("jwt"); // remove if exists
        TimeRemaining = null;

        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(_anonymous)));
    }

    private ClaimsPrincipal CreateClaimsPrincipal(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwt.Claims, "jwt");
        return new ClaimsPrincipal(identity);
    }

    public async Task<string> GetTokenAsync()
        => await _localStorage.GetItemAsync<string>("authToken");

    // ===================== Enhanced Timer =====================

    private void StartTokenExpirationTimer(string token)
    {
        // Cancel previous timer if exists
        _expirationCts?.Cancel();
        _timer?.Stop();
        _timer?.Dispose();
        _expirationCts = new System.Threading.CancellationTokenSource();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
        if (expClaim == null) return;

        var expSeconds = long.Parse(expClaim);
        var expDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

        Console.WriteLine($"[JWT Timer] Token expires at: {expDate:yyyy-MM-dd HH:mm:ss}");

        var delay = expDate - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            Console.WriteLine("[JWT Timer] Token already expired. Logging out immediately.");
            _ = LogoutAsync();
            return;
        }

        // Timer for countdown and auto-logout
        _timer = new System.Timers.Timer(1000); // every second
        _timer.Elapsed += async (s, e) =>
        {
            var remaining = expDate - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
            {
                _timer.Stop();
                Console.WriteLine("Token Expired , user logged out");



                await LogoutAsync();
            }
            else
            {
                TimeRemaining = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
                Console.WriteLine($"[JWT Timer] Time remaining: {remaining.TotalSeconds:N0} sec");
                // Notify UI for optional countdown display
                NotifyAuthenticationStateChanged(Task.FromResult(
                    new AuthenticationState(CreateClaimsPrincipal(token))));
            }
        };
        _timer.Start();
    }


}
