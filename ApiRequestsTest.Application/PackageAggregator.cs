using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;

namespace ApiRequestsTest.Application;

public class PackageAggregator: IPackageAggregator
{
    private readonly IEnumerable<IPackageProvider> _packageProviders;

    public PackageAggregator(IEnumerable<IPackageProvider> packageProviders)
    {
        _packageProviders = packageProviders;
    }

    public async Task<Result<double>> SelectBestPrice(ConsignmentInput input)
    {
        var tasks = _packageProviders.Select(p => ExecuteRequest(p, input));
        var results = await Task.WhenAll(tasks);
        if (results.All(r => r.Failure))
        {
            return Result.Fail<double>("Getting deals all failed!");
        }

        return results.Where(r => r.Success)
            .OrderBy(r => r.Value)
            .First();
    }

    private async Task<Result<double>> ExecuteRequest(IPackageProvider packageProvider, ConsignmentInput input)
    {
        var token = packageProvider.GetAuthenticationCredential();
        var body = packageProvider.SerializeInput(input);
        try
        {
            // Timeout the request if it takes more than 10 secs
            var result = await packageProvider.RequestDealWithRetry(token, body).WaitAsync(TimeSpan.FromSeconds(10));
            return result;
        }
        catch (TimeoutException e)
        {
           return Result.Fail<double>("Getting deal is timeout");
        }
    }
}