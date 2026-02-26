using ERP.Infrastructure.CommonServices;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;

namespace ERP.Tests.CommonServices;
public class WebServiceTests
{
    [Fact]
    public async Task DownloadAsync_ShouldReturnContent_WhenSuccess()
    {
        // Arrange
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"rates\":{}}")
        };

        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockMessageHandler.Object);

        // ✅ Mock NAMED CLIENT (bez extension method)
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                   .Returns(httpClient);

        var service = new WebService(mockFactory.Object);

        // Act
        var result = await service.DownloadAsync("https://test.com");

        // Assert
        result.Should().Be("{\"rates\":{}}");
    }

    [Fact]
    public async Task DownloadAsync_ShouldThrow_WhenNotSuccess()
    {
        // Arrange
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway);

        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(mockMessageHandler.Object);

        // ✅ Mock NAMED CLIENT (bez extension method)
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                   .Returns(httpClient);

        var service = new WebService(mockFactory.Object);

        var act = () => service.DownloadAsync("https://test.com");
        await act.Should().ThrowAsync<Exception>().WithMessage("Web request failed");
    }
}

