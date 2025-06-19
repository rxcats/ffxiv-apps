using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ffxiv_apps;
using ffxiv_apps.Common;
using ffxiv_apps.Game;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetSection("ApiServer").Value!)
});

builder.Services.AddSingleton<Link>();
builder.Services.AddSingleton<World>();
builder.Services.AddSingleton<Player>();
builder.Services.AddSingleton<Controls>();
builder.Services.AddSingleton<Graphics>();

builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();
