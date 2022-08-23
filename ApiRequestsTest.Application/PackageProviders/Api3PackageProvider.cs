using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;

namespace ApiRequestsTest.Application.PackageProviders;

public class Api3PackageProvider: IPackageProvider
{
    public string GetAuthenticationCredential()
    {
        throw new NotImplementedException();
    }

    public string SerializeInput(ConsignmentInput input)
    {
        throw new NotImplementedException();
    }

    public Task<Result<double>> RequestDealWithRetry(string bearerToken, string body)
    {
        throw new NotImplementedException();
    }
}