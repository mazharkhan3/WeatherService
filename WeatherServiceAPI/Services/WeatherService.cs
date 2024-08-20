using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WeatherServiceAPI.Models;

namespace WeatherServiceAPI.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;

    public WeatherService(HttpClient httpClient, IDistributedCache cache, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _cache = cache;
        _configuration = configuration;
    }

    public async Task<WeatherModel?> GetWeather()
    {
        // Try to get data from redis cache
        var cachedData = await _cache.GetStringAsync("WeatherData");

        if (!string.IsNullOrEmpty(cachedData))
        {
            var data = JsonSerializer.Deserialize<WeatherModel>(cachedData);
            return data;
        }
            
        var response = await _httpClient.GetAsync($"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/London%2CUK?unitGroup=us&key={_configuration["WeatherApiKey"]}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<WeatherModel>();
                
            // Serialize the data and store it in Redis cache with a 5-minute expiration
            var serializedData = JsonSerializer.Serialize(data);
            var cachedDataOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync("WeatherData", serializedData, cachedDataOptions);

            return data;
        }

        throw new HttpRequestException($"Request failed with status code {response.StatusCode}");
    }
}
