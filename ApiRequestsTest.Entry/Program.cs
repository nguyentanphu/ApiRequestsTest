// See https://aka.ms/new-console-template for more information

using ApiRequestsTest.Application;
using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Application.PackageProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();
builder.ConfigureServices((hostContext, services) =>
{
    services.AddScoped<IPackageProvider, Api1PackageProvider>()
        .AddHttpClient<IPackageProvider, Api1PackageProvider>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api1:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<IPackageProvider, Api2PackageProvider>()
        .AddHttpClient<IPackageProvider, Api2PackageProvider>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api2:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<IPackageProvider, Api3PackageProvider>()
        .AddHttpClient<IPackageProvider, Api3PackageProvider>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api3:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<Application>();
});
var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;
var app = provider.GetRequiredService<Application>();
var result = app.GetBestDeal();
Console.WriteLine(result.Value);
