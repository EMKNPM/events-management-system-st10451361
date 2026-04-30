using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMaster.Web.Data;
using ContractMaster.Web.Models;
using ContractMaster.Web.Services;

namespace ContractMaster.Web.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContractsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Contracts with Search/Filter
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            // LINQ filtering
            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
                ViewBag.SelectedStatus = status.Value;
            }

            var contracts = await query.ToListAsync();

            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new { Value = (int)s, Name = s.ToString() });

            return View(contracts);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewBag.Clients = _context.Clients.ToList();
            return View();
        }

        // POST: Contracts/Create with PDF upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile SignedAgreement)
        {
            // File validation
            if (SignedAgreement != null)
            {
                var validation = FileValidationService.ValidateFile(SignedAgreement);
                if (!validation.IsValid)
                {
                    ModelState.AddModelError("SignedAgreement", validation.ErrorMessage);
                    ViewBag.Clients = _context.Clients.ToList();
                    return View(contract);
                }

                // Save file
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "contracts");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}_{SignedAgreement.FileName}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await SignedAgreement.CopyToAsync(fileStream);
                }

                contract.SignedAgreementPath = $"/uploads/contracts/{uniqueFileName}";
            }
            else
            {
                ModelState.AddModelError("SignedAgreement", "Please upload a signed agreement (PDF)");
                ViewBag.Clients = _context.Clients.ToList();
                return View(contract);
            }

            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Contract created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = _context.Clients.ToList();
            return View(contract);
        }

        // GET: Contracts/DownloadAgreement
        public async Task<IActionResult> DownloadAgreement(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
            {
                TempData["Error"] = "No signed agreement found.";
                return RedirectToAction(nameof(Index));
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                contract.SignedAgreementPath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "File not found on server.";
                return RedirectToAction(nameof(Index));
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(m => m.ContractId == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
    }
}