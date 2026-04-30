#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMaster.Web.Data;
using ContractMaster.Web.Models;
using ContractMaster.Web.Services;

namespace ContractMaster.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyExchangeService _currencyService;

        public ServiceRequestsController(AppDbContext context, ICurrencyExchangeService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
            return View(requests);
        }

        // GET: ServiceRequests/Create/5
        public async Task<IActionResult> Create(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
            {
                TempData["Error"] = $"Contract with ID {id} not found.";
                return RedirectToAction("Index", "Contracts");
            }

            // WORKFLOW VALIDATION
            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                TempData["Error"] = $"Cannot create service request. Contract is {contract.Status}. Only Active or Draft contracts are eligible.";
                return RedirectToAction("Index", "Contracts");
            }

            var currentRate = await _currencyService.GetUsdToZarRateAsync();

            ViewBag.Contract = contract;
            ViewBag.CurrentRate = currentRate;

            return View(new ServiceRequest { ContractId = contract.ContractId });
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,Description,CostUSD")] ServiceRequest serviceRequest, decimal currentRate)
        {
            var contract = await _context.Contracts.FindAsync(serviceRequest.ContractId);
            if (contract == null)
            {
                ModelState.AddModelError("", "Contract not found.");
                ViewBag.CurrentRate = currentRate;
                return View(serviceRequest);
            }

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError("", $"Cannot create service request. Contract is {contract.Status}.");
                ViewBag.CurrentRate = currentRate;
                ViewBag.Contract = contract;
                return View(serviceRequest);
            }

            // Auto-calculate ZAR amount
            serviceRequest.CostZAR = serviceRequest.CostUSD * currentRate;
            serviceRequest.Status = ServiceRequestStatus.Open;
            serviceRequest.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Service request created! Cost: ${serviceRequest.CostUSD:F2} USD = R{serviceRequest.CostZAR:F2} ZAR";
                return RedirectToAction("Details", "Contracts", new { id = serviceRequest.ContractId });
            }

            ViewBag.CurrentRate = currentRate;
            ViewBag.Contract = contract;
            return View(serviceRequest);
        }
    }
}