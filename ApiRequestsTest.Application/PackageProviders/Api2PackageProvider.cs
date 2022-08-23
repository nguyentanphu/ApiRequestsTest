using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;

namespace ApiRequestsTest.Application.PackageProviders;

public class Api2PackageProvider: IPackageProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Api2PackageProvider(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient(nameof(Api2PackageProvider));
    }

    public string GetAuthenticationCredential()
    {
        // Implement real logic for getting credential
        return $"Bearer {_configuration["ConsignmentApis:Api2:Token"]}";
    }

    public string SerializeInput(ConsignmentInput input)
    {
        var model = new Api2Model
        {
            Consignor = input.SourceAddress,
            Consignee = input.DestinationAddress,
            Cartons = input.Cartons.Select(c => new Api2ModelDimension
            {
                Width = c.Width,
                Height = c.Height,
                Length = c.Length
            }).ToArray()
        };
        return JsonSerializer.Serialize(model, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
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
            return Result.Ok(output.Amount);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<double>(e.Message);
        }
    }

    private class ApiPackageOutput
    {
        public double Amount { get; set; }
    }
}