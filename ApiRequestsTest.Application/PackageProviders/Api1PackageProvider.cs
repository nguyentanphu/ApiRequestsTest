using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;

namespace ApiRequestsTest.Application.PackageProviders;

public class Api1PackageProvider: IPackageProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Api1PackageProvider(IConfiguration configuration, IHttpClientFactory clientFactory)
    {
        _configuration = configuration;
        _httpClient = clientFactory.CreateClient(nameof(Api1PackageProvider));
    }

    public string GetAuthenticationCredential()
    {
        // Implement real logic for getting credential
        return $"Bearer {_configuration["ConsignmentApis:Api1:Token"]}";
    }

    public string SerializeInput(ConsignmentInput input)
    {
        var model = new Api1Model
        {
            ContactAddress = input.SourceAddress,
            WarehouseAddress = input.DestinationAddress,
            PackageDimensions = input.Cartons.Select(c => new Api1ModelDimension
            {
                Width = c.Width,
                Height = c.Height,
                Length = c.Length
            }).ToArray()
        };
        return JsonSerializer.Serialize(model);
    }

    public async Task<Result<double>> RequestDealWithRetry(string bearerToken, string serializedData)
    {
        _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
        var body = new StringContent(serializedData, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("", body);
        try
        {
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var output = JsonSerializer.Deserialize<ApiPackageOutput>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return Result.Ok(output.Total);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<double>(e.Message);
        }
    }

    private class ApiPackageOutput
    {
        public double Total { get; set; }
    }
}