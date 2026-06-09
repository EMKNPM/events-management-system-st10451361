using System.Text;
using System.Text.Json;
using ContractMaster.Web.Models;

namespace ContractMaster.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _token;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");

            if (!string.IsNullOrEmpty(_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var loginData = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResponse>(json);
            return result?.token ?? string.Empty;
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            var response = await _httpClient.GetAsync("api/clients");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
        }

        public async Task<Client> GetClientAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/clients/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json) ?? new Client();
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            var content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/clients", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json) ?? client;
        }

        public async Task UpdateClientAsync(int id, Client client)
        {
            var content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/clients/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClientAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var query = new List<string>();
            if (startDate.HasValue) query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue) query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            if (status.HasValue) query.Add($"status={(int)status.Value}");

            var url = "api/contracts";
            if (query.Any()) url += "?" + string.Join("&", query);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Contract>>(json) ?? new List<Contract>();
        }

        public async Task<Contract> GetContractAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/contracts/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contract>(json) ?? new Contract();
        }

        public async Task<Contract> CreateContractAsync(Contract contract)
        {
            var content = new StringContent(JsonSerializer.Serialize(contract), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/contracts", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contract>(json) ?? contract;
        }

        public async Task UpdateContractAsync(int id, Contract contract)
        {
            var content = new StringContent(JsonSerializer.Serialize(contract), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/contracts/{id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateContractStatusAsync(int id, ContractStatus status)
        {
            var content = new StringContent(JsonSerializer.Serialize(status), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/contracts/{id}/status", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteContractAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            var response = await _httpClient.GetAsync("api/servicerequests");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json) ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest> GetServiceRequestAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/servicerequests/{id}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json) ?? new ServiceRequest();
        }

        public async Task<ServiceRequest> CreateServiceRequestAsync(int contractId, string description, decimal costUSD)
        {
            var requestDto = new { contractId, description, costUSD };
            var content = new StringContent(JsonSerializer.Serialize(requestDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/servicerequests", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json) ?? new ServiceRequest();
        }

        public async Task UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus status)
        {
            var content = new StringContent(JsonSerializer.Serialize(status), Encoding.UTF8, "application/json");
            var response = await _httpClient.PatchAsync($"api/servicerequests/{id}/status", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<decimal> GetExchangeRateAsync()
        {
            var response = await _httpClient.GetAsync("api/servicerequests/rate");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ExchangeRateResponse>(json);
            return result?.usdToZar ?? 19.50m;
        }
    }

    internal class LoginResponse { public string token { get; set; } = string.Empty; }
    internal class ExchangeRateResponse { public decimal usdToZar { get; set; } }
}