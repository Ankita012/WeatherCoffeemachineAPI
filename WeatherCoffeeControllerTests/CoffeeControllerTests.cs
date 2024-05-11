using System.Net;
using System;
using WeatherCoffeeMachineAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace WeatherCoffeeControllerTests
{ 
    public class CoffeeControllerTests
    {
        [Fact]
        public async Task BrewCoffee_ReturnsStatusCode418_OnAprilFirst()
        {
            // Arrange
            var controller = new CoffeeController();

            // Act
            var result = await controller.BrewCoffee(new DateTime(2024, 4, 1)) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(418, result.StatusCode);
            Assert.Equal("I'm a teapot", result.Value);
        }

        [Fact]
        public async Task BrewCoffee_ReturnsStatusCode503_OnFifthCall()
        {
            // Arrange
            var controller = new CoffeeController();

            // Act & Assert multiple times to test the counter logic
            for (int i = 1; i <= 5; i++)
            {
                var result = await controller.BrewCoffee() as ObjectResult;
                if (i < 5)
                {
                    Assert.Equal(200, result?.StatusCode);
                }
                else
                {
                    Assert.Equal(503, result?.StatusCode);
                    Assert.Equal("Service Unavailable", result?.Value);
                }
            }
        }

        [Fact]
        public async Task BrewCoffee_ReturnsOkResult_WhenTemperatureIsBelow30()
        {
            // Arrange
            var controller = new CoffeeController();
            var weatherResponse = "{\"main\":{\"temp\":25.0}}";
            var mockHttpClient = new Mock<IHttpClientFactory>();
            mockHttpClient.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new MockHttpMessageHandler(weatherResponse)));

            controller._httpClientFactory = mockHttpClient.Object;

            // Act
            var result = await controller.BrewCoffee(temp: 25.0) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseValue = result.Value;
            Assert.NotNull(responseValue);
            Assert.True(responseValue.GetType().GetProperty("message") != null);
            Assert.Equal("Your piping hot coffee is ready", responseValue?.GetType()?.GetProperty("message")?.GetValue(responseValue));
        }

        [Fact]
        public async Task BrewCoffee_ReturnsOkResult_WhenTemperatureIsAbove30()
        {
            // Arrange
            var controller = new CoffeeController();
            var weatherResponse = "{\"main\":{\"temp\":31.0}}";
            var mockHttpClient = new Mock<IHttpClientFactory>();
            mockHttpClient.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient(new MockHttpMessageHandler(weatherResponse)));

            // Act
            var result = await controller.BrewCoffee(temp: 31.0) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseValue = result.Value;
            Assert.NotNull(responseValue);
            Assert.True(responseValue.GetType().GetProperty("message") != null);
            Assert.Equal("Your refreshing iced coffee is ready", responseValue?.GetType()?.GetProperty("message")?.GetValue(responseValue));
        }
    
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;

        public MockHttpMessageHandler(string response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(_response) };
            return Task.FromResult(response);
        }
    }
}
