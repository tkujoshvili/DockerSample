using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace DockerSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet("Weathers")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var weathers = new List<WeatherForecast>();
            await using var connection = new MySqlConnection(Environment.GetEnvironmentVariable("ConnectionString"));

            await connection.OpenAsync();

            await using var command = new MySqlCommand("SELECT * FROM weathers;", connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                weathers.Add(new WeatherForecast()
                {
                    Date = reader.GetDateTime(0),
                    TemperatureC = reader.GetInt32(1),
                    Summary = reader.GetString(2)
                });
            }

            return weathers;
        }

        [HttpGet("SaveRandomWeather")]
        public async Task<WeatherForecast> SaveRandomWeather()
        {
            var weather = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(Random.Shared.Next(5)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            };

            await using var connection = new MySqlConnection(Environment.GetEnvironmentVariable("ConnectionString"));
            const string insertData = "insert into weathers(date,temperature,summery) values (@date, @temperature, @summery)";
            var command = new MySqlCommand(insertData, connection);

            command.Parameters.AddWithValue("@date", weather.Date);
            command.Parameters.AddWithValue("@temperature", weather.TemperatureC);
            command.Parameters.AddWithValue("@summery", weather.Summary);

            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();

            return weather;
        }
    }
}