using System.Text.Json;

namespace ContractMaster.API.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CurrencyExchangeService> _logger;

        public CurrencyExchangeService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<CurrencyExchangeService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var apiKey = _configuration["ExchangeRateApi:Key"];
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_FREE_API_KEY_HERE")
                {
                    return 19.50m;
                }

                var url = $"https://v6.exchangerate-api.com/v6/{apiKey}/pair/USD/ZAR";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(json);
                var rate = document.RootElement.GetProperty("conversion_rate").GetDecimal();

                return rate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get exchange rate");
                return 19.50m;
            }
        }
    }
}