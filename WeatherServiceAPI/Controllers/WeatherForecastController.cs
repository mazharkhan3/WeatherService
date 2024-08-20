using Microsoft.AspNetCore.Mvc;
using WeatherServiceAPI.Models;
using WeatherServiceAPI.Services;

namespace WeatherServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherForecastController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<WeatherModel?> Get()
        {
            return await _weatherService.GetWeather();
        }
    }
}
