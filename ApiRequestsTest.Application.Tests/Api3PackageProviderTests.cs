using System.Net;
using System.Text.Json;
using ApiRequestsTest.Application.PackageProviders;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApiRequestsTest.Application.Tests;

public class Api3PackageProviderTests
{
    private readonly Mock<IHttpClientFactory> _factoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
    private readonly IConfiguration _configuration;

    public Api3PackageProviderTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConsignmentApis:Api3:Token", "Test token" },
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build() as IConfiguration;
    }

    [Fact]
    public void GetAuthenticationCredential_Succeed()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStub());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api3PackageProvider)))
            .Returns(httpClient);

        var subject = new Api3PackageProvider(_configuration, _factoryMock.Object);
        var result = subject.GetAuthenticationCredential();
        Assert.Equal("Bearer Test token", result);
    }

    [Fact]
    public void SerializeInput_Succeed()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStub());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api3PackageProvider)))
            .Returns(httpClient);

        var subject = new Api3PackageProvider(_configuration, _factoryMock.Object);
        var result = subject.SerializeInput(new ConsignmentInput
        {
            SourceAddress = "123",
            DestinationAddress = "456",
            Cartons = new[]
            {
                new CartonDimension
                {
                    Width = 22,
                    Height = 44,
                    Length = 33
                }
            }
        });
        Assert.Equal(
            "<?xml version=\"1.0\" encoding=\"utf-16\"?><PackageInput xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Source>123</Source><Destination>456</Destination><Packages><Api3ModelDimension><Width>22</Width><Length>33</Length><Height>44</Height></Api3ModelDimension></Packages></PackageInput>",
            result);
    }

    [Fact]
    public async Task RequestDealWithRetry_SucceedResponseFromApi()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStub());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api3PackageProvider)))
            .Returns(httpClient);

        var subject = new Api3PackageProvider(_configuration, _factoryMock.Object);
        var result = await subject.RequestDealWithRetry("Test token", "Test Input");

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RequestDealWithRetry_ApiReturnInvalidJson_CauseJsonException()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStubWithInvalidXml());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api3PackageProvider)))
            .Returns(httpClient);

        var subject = new Api3PackageProvider(_configuration, _factoryMock.Object);
        var result = Assert.ThrowsAsync<Exception>(async () =>
            await subject.RequestDealWithRetry("Test token", "Test Input")
        );

        Assert.IsType<InvalidOperationException>(result.Exception.InnerException.InnerException);
    }

    private class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public DelegatingHandlerStub()
        {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("<PackageQuote xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Quote>2.947431987130511</Quote></PackageQuote>")
            });
        }

        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }

    private class DelegatingHandlerStubWithInvalidXml : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

        public DelegatingHandlerStubWithInvalidXml()
        {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid xml")
            });
        }

        public DelegatingHandlerStubWithInvalidXml(
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
        {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }
}