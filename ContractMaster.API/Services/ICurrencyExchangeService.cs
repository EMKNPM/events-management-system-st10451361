namespace ContractMaster.API.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> GetUsdToZarRateAsync();
    }
}