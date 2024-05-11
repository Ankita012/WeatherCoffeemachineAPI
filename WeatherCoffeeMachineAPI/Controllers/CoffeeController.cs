using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WeatherCoffeeMachineAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {
        private static int brewCounter = 0;
        private readonly HttpClient httpClient;
        public IHttpClientFactory _httpClientFactory;

        public CoffeeController()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "C# console program");
        }

        [HttpGet("brew-coffee")]
        public async Task<IActionResult> BrewCoffee(DateTime? dateTime = null, double? temp = null)
        {
            if (dateTime == null)
            {
                dateTime = DateTime.UtcNow;
            }

            if (dateTime.Value.Month == 4 && dateTime.Value.Day == 1)
            {
                return StatusCode(418, "I'm a teapot");
            }

            brewCounter++;
            if (brewCounter % 5 == 0)
            {
                return StatusCode(503, "Service Unavailable");
            }

            if (temp.HasValue)
            {
                temp = temp.Value;
            }
            else
            {
                var weatherResponse = await GetWeatherAsync();
                temp = await GetTemperatureAsync(weatherResponse);
            }

            var message = temp > 30 ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready";

            var response = new
            {
                message,
                prepared = dateTime.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")
            };
            return Ok(response);
        }

        private async Task<string> GetWeatherAsync()
        {
            var response = await httpClient.GetStringAsync("https://api.openweathermap.org/data/2.5/weather?lat=20.59&lon=78.96&appid=67b685fcdf9bf5cf8353f8a79ebc1eb4&units=metric");
            return response;
        }

        private async Task<double> GetTemperatureAsync(string weatherResponse)
        {
            var weatherData = JsonConvert.DeserializeObject<WeatherData>(weatherResponse);
            return weatherData.Main.Temp;
        }
    }

    public class WeatherData
    {
        public Main Main { get; set; }
    }

    public class Main
    {
        public double Temp { get; set; }
    }
}