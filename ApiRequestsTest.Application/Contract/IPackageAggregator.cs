using ApiRequestsTest.Domain;

namespace ApiRequestsTest.Application.Contract;

public interface IPackageAggregator
{
    Task<Result<double>> SelectBestPrice(ConsignmentInput input);
}