using ApiRequestsTest.Domain;

namespace ApiRequestsTest.Application.Contract;

public interface IPackageProvider
{
    public string GetAuthenticationCredential();
    public string SerializeInput(ConsignmentInput input);
    public Task<Result<double>> RequestDealWithRetry(string bearerToken, string body);
}