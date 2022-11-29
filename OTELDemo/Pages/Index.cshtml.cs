using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using OTELDemo.Model;

namespace OTELDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IEnumerable<WeatherForecast> Forecasts { get; private set; } = new WeatherForecast[] { };

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            var apiUri = new Uri(Configuration.WebApiUrl);
            var forecastUrl = new Uri(apiUri, "WeatherForecast");

            // Наш http client уже инструментирован OpenTelemetry в Program.cs
            // все запросы буду содержать дополнительный заголовок с контекстом трассировки
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(forecastUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Requested weather forecast from API");
            Forecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(responseBody) ?? new WeatherForecast[] { };
        }
    }
}