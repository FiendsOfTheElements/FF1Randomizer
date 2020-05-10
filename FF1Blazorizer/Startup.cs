using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazor.Extensions.Storage;

namespace FF1Blazorizer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
	        services.AddStorage();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
