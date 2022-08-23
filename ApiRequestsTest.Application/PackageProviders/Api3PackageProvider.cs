using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;

namespace ApiRequestsTest.Application.PackageProviders;

public class Api3PackageProvider: IPackageProvider
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Api3PackageProvider(IConfiguration configuration, IHttpClientFactory clientFactory)
    {
        _configuration = configuration;
        _httpClient = clientFactory.CreateClient(nameof(Api3PackageProvider));
    }

    public string GetAuthenticationCredential()
    {
        // Implement real logic for getting credential
        return $"Bearer {_configuration["ConsignmentApis:Api3:Token"]}";
    }

    public string SerializeInput(ConsignmentInput input)
    {
        var model = new Api3Model
        {
            Source = input.SourceAddress,
            Destination = input.DestinationAddress,
            Packages = input.Cartons.Select(c => new Api3ModelDimension
            {
                Width = c.Width,
                Height = c.Height,
                Length = c.Length
            }).ToArray()
        };
        var xmlSerializer = new XmlSerializer(model.GetType());
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, model);
        return stringWriter.ToString();
    }

    public async Task<Result<double>> RequestDealWithRetry(string bearerToken, string serializedData)
    {
        _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(bearerToken);
        var body = new StringContent(serializedData, Encoding.Unicode, "application/xml");
        var response = await _httpClient.PostAsync("", body);
        try
        {
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            using var stringReader = new StringReader(result);
            var xmlSerializer = new XmlSerializer(typeof(ApiPackageOutput));
            var data = (ApiPackageOutput)xmlSerializer.Deserialize(stringReader)!;
            return Result.Ok(data.Quote);
        }
        catch (HttpRequestException e)
        {
            return Result.Fail<double>(e.Message);
        }
    }

    [XmlRoot(ElementName = "PackageQuote")]
    public class ApiPackageOutput
    {
        public double Quote { get; set; }
    }
}