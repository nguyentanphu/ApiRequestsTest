using ApiRequestsTest.Application.Contract;
using ApiRequestsTest.Domain;
using Moq;
using Xunit;

namespace ApiRequestsTest.Application.Tests;

public class PackageAggregatorTests
{
    private readonly Mock<IEnumerable<IPackageProvider>> _packageProvidersMock;

    public PackageAggregatorTests()
    {
        _packageProvidersMock = new Mock<IEnumerable<IPackageProvider>>();
    }

    [Fact]
    public async Task SelectBestPrice_AllProvidersFailed()
    {
        var api1 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api1.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api1 Test serialized input");
        api1.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api1 Test bearer token");
        api1.Setup(p => p.RequestDealWithRetry("Api1 Test bearer token", "Api1 Test serialized input"))
            .ReturnsAsync(Result.Fail<double>("Api1 failed"));
        
        var api2 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api2.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api2 Test serialized input");
        api2.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api2 Test bearer token");
        api2.Setup(p => p.RequestDealWithRetry("Api2 Test bearer token", "Api2 Test serialized input"))
            .ReturnsAsync(Result.Fail<double>("Api2 failed"));

        var providers = new[]
        {
            api1.Object,
            api2.Object
        };
        var testSubject = new PackageAggregator(providers);

        var result = await testSubject.SelectBestPrice(new ConsignmentInput
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
        
        Assert.True(result.Failure);
        Assert.Equal("Getting deals all failed!", result.Error);
    }
    
    [Fact]
    public async Task SelectBestPrice_ApiTakeMoreThan10SecsToComplete_FailedAsTimeout()
    {
        var api1 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api1.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api1 Test serialized input");
        api1.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api1 Test bearer token");
        api1.Setup(p => p.RequestDealWithRetry("Api1 Test bearer token", "Api1 Test serialized input"))
            .Returns(async () =>
            {
                await Task.Delay(15000);
                return Result.Ok<double>(15.5);
            });
        

        var providers = new[]
        {
            api1.Object
        };
        var testSubject = new PackageAggregator(providers);

        var result = await testSubject.SelectBestPrice(new ConsignmentInput
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
        
        Assert.True(result.Failure);
        Assert.Equal("Getting deals all failed!", result.Error);
    }
    
    [Fact]
    public async Task SelectBestPrice_SomeApisFailed_OkResult()
    {
        var api1 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api1.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api1 Test serialized input");
        api1.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api1 Test bearer token");
        api1.Setup(p => p.RequestDealWithRetry("Api1 Test bearer token", "Api1 Test serialized input"))
            .ReturnsAsync(Result.Fail<double>("Api1 failed"));
        
        var api2 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api2.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api2 Test serialized input");
        api2.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api2 Test bearer token");
        api2.Setup(p => p.RequestDealWithRetry("Api2 Test bearer token", "Api2 Test serialized input"))
            .ReturnsAsync(Result.Fail<double>("Api2 failed"));
        
        var api3 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api3.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api3 Test serialized input");
        api3.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api3 Test bearer token");
        api3.Setup(p => p.RequestDealWithRetry("Api3 Test bearer token", "Api3 Test serialized input"))
            .ReturnsAsync(Result.Ok(33.55));
        
        var api4 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api4.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api4 Test serialized input");
        api4.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api4 Test bearer token");
        api4.Setup(p => p.RequestDealWithRetry("Api4 Test bearer token", "Api4 Test serialized input"))
            .ReturnsAsync(Result.Ok(40.44));

        var providers = new[]
        {
            api1.Object,
            api2.Object,
            api3.Object,
            api4.Object
        };
        var testSubject = new PackageAggregator(providers);

        var result = await testSubject.SelectBestPrice(new ConsignmentInput
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
        
        Assert.True(result.Success);
        Assert.Equal(33.55, result.Value);
    }
    
    [Fact]
    public async Task SelectBestPrice_AllApiSucceeded_OkResult()
    {
        var api1 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api1.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api1 Test serialized input");
        api1.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api1 Test bearer token");
        api1.Setup(p => p.RequestDealWithRetry("Api1 Test bearer token", "Api1 Test serialized input"))
            .ReturnsAsync(Result.Ok(10.44));
        
        var api2 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api2.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api2 Test serialized input");
        api2.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api2 Test bearer token");
        api2.Setup(p => p.RequestDealWithRetry("Api2 Test bearer token", "Api2 Test serialized input"))
            .ReturnsAsync(Result.Ok(9.34));
        
        var api3 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api3.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api3 Test serialized input");
        api3.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api3 Test bearer token");
        api3.Setup(p => p.RequestDealWithRetry("Api3 Test bearer token", "Api3 Test serialized input"))
            .ReturnsAsync(Result.Ok(33.55));
        
        var api4 = new Mock<IPackageProvider>(MockBehavior.Strict);
        api4.Setup(p => p.SerializeInput(It.IsAny<ConsignmentInput>()))
            .Returns("Api4 Test serialized input");
        api4.Setup(p => p.GetAuthenticationCredential())
            .Returns("Api4 Test bearer token");
        api4.Setup(p => p.RequestDealWithRetry("Api4 Test bearer token", "Api4 Test serialized input"))
            .ReturnsAsync(Result.Ok(40.44));

        var providers = new[]
        {
            api1.Object,
            api2.Object,
            api3.Object,
            api4.Object
        };
        var testSubject = new PackageAggregator(providers);

        var result = await testSubject.SelectBestPrice(new ConsignmentInput
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
        
        Assert.True(result.Success);
        Assert.Equal(9.34, result.Value);
    }
}