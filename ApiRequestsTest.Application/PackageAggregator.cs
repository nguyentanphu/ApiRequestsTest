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

    public async Task<Result<double>> SelectBestDeal(ConsignmentInput input)
    {
        var tasks = _packageProviders.Select(p => ExecuteDeal(p, input));
        var results = await Task.WhenAll(tasks);
        if (results.All(r => r.Failure))
        {
            return Result.Fail<double>("Getting deals all failed!");
        }

        var minPrice = results.Where(r => r.Success)
            .Min(r => r.Value);
        return Result.Ok<double>(minPrice);
    }

    private async Task<Result<double>> ExecuteDeal(IPackageProvider packageProvider, ConsignmentInput input)
    {
        var token = packageProvider.GetAuthenticationCredential();
        var body = packageProvider.SerializeInput(input);
        try
        {
            var result = await packageProvider.RequestDealWithRetry(token, body).WaitAsync(TimeSpan.FromSeconds(10));
            return result;
        }
        catch (TimeoutException e)
        {
           return Result.Fail<double>("Getting deal is timeout");
        }
    }
}