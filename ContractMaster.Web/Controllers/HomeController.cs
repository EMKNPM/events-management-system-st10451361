using ContractMaster.Web.Models;
using ContractMaster.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractMaster.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        public HomeController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetClientsAsync();
            var contracts = await _apiService.GetContractsAsync(null, null, null);

            ViewBag.TotalClients = clients.Count;
            ViewBag.TotalContracts = contracts.Count;
            ViewBag.ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}