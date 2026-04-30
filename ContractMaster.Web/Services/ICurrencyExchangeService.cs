namespace ContractMaster.Web.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> GetUsdToZarRateAsync();
    }
}