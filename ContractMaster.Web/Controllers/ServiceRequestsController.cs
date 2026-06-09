using Microsoft.AspNetCore.Mvc;
using ContractMaster.Web.Models;
using ContractMaster.Web.Services;

namespace ContractMaster.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IApiService _apiService;

        public ServiceRequestsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _apiService.GetServiceRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _apiService.GetContractAsync(contractId);
            if (contract == null)
            {
                TempData["Error"] = "Contract not found.";
                return RedirectToAction("Index", "Contracts");
            }

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                TempData["Error"] = $"Cannot create service request. Contract is {contract.Status}.";
                return RedirectToAction("Index", "Contracts");
            }

            var currentRate = await _apiService.GetExchangeRateAsync();
            ViewBag.Contract = contract;
            ViewBag.CurrentRate = currentRate;
            return View(new ServiceRequest { ContractId = contractId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,Description,CostUSD")] ServiceRequest serviceRequest, decimal currentRate)
        {
            var created = await _apiService.CreateServiceRequestAsync(serviceRequest.ContractId, serviceRequest.Description, serviceRequest.CostUSD);
            TempData["Success"] = $"Service request created! Cost: ${created.CostUSD} USD = R{created.CostZAR:F2} ZAR";
            return RedirectToAction("Details", "Contracts", new { id = serviceRequest.ContractId });
        }
    }
}