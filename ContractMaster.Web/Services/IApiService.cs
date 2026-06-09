using ContractMaster.Web.Models;

namespace ContractMaster.Web.Services
{
    public interface IApiService
    {
        Task<string> LoginAsync(string username, string password);
        Task<List<Client>> GetClientsAsync();
        Task<Client> GetClientAsync(int id);
        Task<Client> CreateClientAsync(Client client);
        Task UpdateClientAsync(int id, Client client);
        Task DeleteClientAsync(int id);

        Task<List<Contract>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task<Contract> GetContractAsync(int id);
        Task<Contract> CreateContractAsync(Contract contract);
        Task UpdateContractAsync(int id, Contract contract);
        Task UpdateContractStatusAsync(int id, ContractStatus status);
        Task DeleteContractAsync(int id);

        Task<List<ServiceRequest>> GetServiceRequestsAsync();
        Task<ServiceRequest> GetServiceRequestAsync(int id);
        Task<ServiceRequest> CreateServiceRequestAsync(int contractId, string description, decimal costUSD);
        Task UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus status);

        Task<decimal> GetExchangeRateAsync();
    }
}