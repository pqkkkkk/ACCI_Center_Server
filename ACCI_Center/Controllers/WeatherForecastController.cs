using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers;

[ApiController]
//[Route("[controller]")]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    [HttpPost(Name = "PostWeatherForecast")]
    public IActionResult Post([FromBody] WeatherForecast weatherForecast)
    {
        if (weatherForecast == null)
        {
            return BadRequest("Weather forecast data is required.");
        }
        // Here you would typically save the weather forecast to a database or perform some action.
        // For this example, we will just log it and return a success response.
        _logger.LogInformation("Received weather forecast: {@WeatherForecast}", weatherForecast);
        return Ok("Weather forecast received successfully.");
    }
}
