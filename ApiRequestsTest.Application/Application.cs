using Microsoft.Extensions.Configuration;

namespace ApiRequestsTest.Application;

public class Application
{
    private readonly IConfiguration _configuration;

    public Application(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Result<double> GetBestDeal()
    {
        Console.WriteLine(_configuration["Urls:Api1"]);
        return Result.Ok(4.555);
    }
}