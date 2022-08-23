using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;

namespace ApiRequestsTest.Application;

public class Application
{
    private readonly IPackageAggregator _packageAggregator;

    public Application(IPackageAggregator packageAggregator)
    {
        _packageAggregator = packageAggregator;
    }

    public Task<Result<double>> GetBestDeal(ConsignmentInput input)
    {
        return _packageAggregator.SelectBestPrice(input);
    }
}