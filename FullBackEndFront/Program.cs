using Blazored.LocalStorage;
using EmployeeManagmentClient;
using EmployeeManagmentClient.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthStateProvider>(sp =>
    (JwtAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7237/") });

builder.Services.AddScoped<IpServices>();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiClient>();


await builder.Build().RunAsync();
