using CinemaRazor;
using CinemaRazor.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5190/") });

builder.Services.AddSingleton<AuthState>();
builder.Services.AddSingleton<Toast>();
builder.Services.AddScoped<Api>();
builder.Services.AddScoped<HubService>();

await builder.Build().RunAsync();
