using Microsoft.AspNetCore.Blazor.Hosting;
using BlazorStrap;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FF1Blazorizer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

			IWebAssemblyHostBuilder builder = ;

			builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
			builder.Services.AddBootstrapCss();
			builder.Services.AddBlazorPrettyCode(defaults =>
			{
				defaults.DefaultTheme = "SolarizedDark";
				defaults.ShowLineNumbers = true;
			});

			builder.RootComponents.Add<App>("app");

			await builder.Build().RunAsync();
		}

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
