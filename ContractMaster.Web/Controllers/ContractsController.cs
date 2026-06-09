using Microsoft.AspNetCore.Mvc;
using ContractMaster.Web.Models;
using ContractMaster.Web.Services;

namespace ContractMaster.Web.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IApiService _apiService;

        public ContractsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _apiService.GetContractsAsync(startDate, endDate, status);
            return View(contracts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateContractAsync(contract);
                TempData["Success"] = "Contract created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(contract);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contract = await _apiService.GetContractAsync(id.Value);
            if (contract == null) return NotFound();
            return View(contract);
        }
    }
}