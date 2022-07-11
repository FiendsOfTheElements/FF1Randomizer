global using System;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using System.Text;

using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using BlazorStrap;
using Blazored.LocalStorage;
using FF1Blazorizer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBootstrapCss();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
