using Microsoft.AspNetCore.Mvc;
using ContractMaster.Web.Models;
using ContractMaster.Web.Services;

namespace ContractMaster.Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IApiService _apiService;

        public ClientsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetClientsAsync();
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                await _apiService.CreateClientAsync(client);
                TempData["Success"] = "Client created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
    }
}