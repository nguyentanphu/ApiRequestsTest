// See https://aka.ms/new-console-template for more information

using ApiRequestsTest.Application;
using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Application.PackageProviders;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();
builder.ConfigureServices((hostContext, services) =>
{
    services.AddScoped<IPackageProvider, Api1PackageProvider>()
        .AddHttpClient(nameof(Api1PackageProvider), client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api1:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<IPackageProvider, Api2PackageProvider>()
        .AddHttpClient(nameof(Api2PackageProvider), client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api2:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<IPackageProvider, Api3PackageProvider>()
        .AddHttpClient(nameof(Api3PackageProvider), client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ConsignmentApis:Api3:Url"]);
        })
        .AddPolicyHandler(ConsignmentRetryPolicy.GetRetryPolicy());
    services.AddScoped<IPackageAggregator, PackageAggregator>();
    services.AddScoped<IApplication, Application>();
});
var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;
var app = provider.GetRequiredService<IApplication>();
Console.WriteLine("Press any key to find best consignment deal!");
Console.ReadKey();
var input = new ConsignmentInput
{
    SourceAddress = "94783 154 St Surrey",
    DestinationAddress = "3455 96 Ave, Surrey",
    Cartons = new[]
    {
        new CartonDimension
        {
            Width = 22,
            Height = 11,
            Length = 33
        },
        new CartonDimension
        {
            Width = 456,
            Height = 789,
            Length = 999
        }
    }
};

var result = await app.GetBestDeal(input);
if (result.Success)
{
    Console.WriteLine($"Best deal is: ${result.Value}.");
}
else
{
    Console.WriteLine("Cannot find any deals as all apis are failed.");
}