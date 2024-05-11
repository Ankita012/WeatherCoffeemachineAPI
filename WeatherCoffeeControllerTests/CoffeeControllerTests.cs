using System;
using WeatherCoffeeMachineAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

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
}
