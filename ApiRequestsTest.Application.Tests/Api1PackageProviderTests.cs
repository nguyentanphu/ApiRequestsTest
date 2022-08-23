using System.Net;
using System.Text.Json;
using ApiRequestsTest.Application.PackageProviders;
using ApiRequestsTest.Domain;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApiRequestsTest.Application.Tests;

public class Api1PackageProviderTests
{
    private readonly Mock<IHttpClientFactory> _factoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);
    private readonly IConfiguration _configuration;

    public Api1PackageProviderTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"ConsignmentApis:Api1:Token", "Test token"},
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
        _factoryMock.Setup(f => f.CreateClient(nameof(Api1PackageProvider)))
            .Returns(httpClient);

        var subject = new Api1PackageProvider(_configuration, _factoryMock.Object);
        var result = subject.GetAuthenticationCredential();
        Assert.Equal("Bearer Test token", result);
    }

    [Fact]
    public void SerializeInput_Succeed()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStub());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api1PackageProvider)))
            .Returns(httpClient);

        var subject = new Api1PackageProvider(_configuration, _factoryMock.Object);
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
        Assert.Equal("{\"ContactAddress\":\"123\",\"WarehouseAddress\":\"456\",\"PackageDimensions\":[{\"Width\":22,\"Length\":33,\"Height\":44}]}", result);
    }

    [Fact]
    public async Task RequestDealWithRetry_SucceedResponseFromApi()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStub());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api1PackageProvider)))
            .Returns(httpClient);

        var subject = new Api1PackageProvider(_configuration, _factoryMock.Object);
        var result = await subject.RequestDealWithRetry("Test token", "Test Input");
        
        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task RequestDealWithRetry_ApiReturnInvalidJson_CauseJsonException()
    {
        var httpClient = new HttpClient(new DelegatingHandlerStubWithInvalidJson());
        httpClient.BaseAddress = new Uri("http://localhost");
        _factoryMock.Setup(f => f.CreateClient(nameof(Api1PackageProvider)))
            .Returns(httpClient);

        var subject = new Api1PackageProvider(_configuration, _factoryMock.Object);
        var result = Assert.ThrowsAsync<Exception>( async () =>
             await subject.RequestDealWithRetry("Test token", "Test Input")
        );
        
        Assert.IsType<JsonException>(result.Exception.InnerException.InnerException);
    }
    
    private class DelegatingHandlerStub : DelegatingHandler {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStub() {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ContactAddress\": \"456 Street\"}")
            });
        }

        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return _handlerFunc(request, cancellationToken);
        }
    }

    private class DelegatingHandlerStubWithInvalidJson : DelegatingHandler {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStubWithInvalidJson() {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("545454")
            });
        }

        public DelegatingHandlerStubWithInvalidJson(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {
            _handlerFunc = handlerFunc;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return _handlerFunc(request, cancellationToken);
        }
    }
}

