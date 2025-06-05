using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SpawnDev.BlazorJS;

namespace SudokuBlazorUI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        
        // Add SpawnDev.BlazorJS.BlazorJSRuntime
        builder.Services.AddBlazorJSRuntime();

        // build and Init using BlazorJSRunAsync (instead of RunAsync)
        await builder.Build().BlazorJSRunAsync();
    }
}
