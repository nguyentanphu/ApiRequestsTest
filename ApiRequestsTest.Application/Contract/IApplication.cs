using ApiRequestsTest.Domain;

namespace ApiRequestsTest.Application.Contract;

public interface IApplication
{
    Task<Result<double>> GetBestDeal(ConsignmentInput input);
}