using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace gymapi.UnitTests.Utility;

public static class MockHttpMessageHandler<T>
{
    public static Mock<HttpMessageHandler> SetupBasicGetResourceList(List<T> expectedResponse)
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
        };


        mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(mockResponse);

        return handlerMock;
    }

    public static Mock<HttpMessageHandler> SetupBasicGetResourceList(List<T> expectedResponse, string endpoint)
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
        };


        mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var handlerMock = new Mock<HttpMessageHandler>();

        var HttpRequestMessage = new HttpRequestMessage()
        {
            RequestUri = new Uri(endpoint),
            Method = HttpMethod.Get
        };

        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        return handlerMock;
    }

    public static Mock<HttpMessageHandler> SetupReturn404()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("")
        };
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(mockResponse);

        return handlerMock;

    }

}