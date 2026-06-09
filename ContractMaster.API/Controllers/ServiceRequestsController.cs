using ContractMaster.API.Data;
using ContractMaster.API.Models;
using ContractMaster.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractMaster.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyExchangeService _currencyService;

        public ServiceRequestsController(AppDbContext context, ICurrencyExchangeService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        // GET: api/servicerequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetServiceRequests()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c.Client)
                .ToListAsync();
        }

        // GET: api/servicerequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return serviceRequest;
        }

        // POST: api/servicerequests
        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> PostServiceRequest(ServiceRequestDto requestDto)
        {
            // Verify contract exists and is valid
            var contract = await _context.Contracts.FindAsync(requestDto.ContractId);
            if (contract == null)
            {
                return BadRequest("Contract not found");
            }

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                return BadRequest($"Cannot create service request. Contract is {contract.Status}");
            }

            // Get current exchange rate
            var currentRate = await _currencyService.GetUsdToZarRateAsync();

            var serviceRequest = new ServiceRequest
            {
                ContractId = requestDto.ContractId,
                Description = requestDto.Description,
                CostUSD = requestDto.CostUSD,
                CostZAR = requestDto.CostUSD * currentRate,
                Status = ServiceRequestStatus.Open,
                CreatedAt = DateTime.Now
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceRequest), new { id = serviceRequest.ServiceRequestId }, serviceRequest);
        }

        // PATCH: api/servicerequests/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, [FromBody] ServiceRequestStatus status)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            serviceRequest.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { id = id, status = status.ToString() });
        }

        // GET: api/servicerequests/rate
        [HttpGet("rate")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetExchangeRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Ok(new { usdToZar = rate, timestamp = DateTime.Now });
        }
    }

    public class ServiceRequestDto
    {
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
    }
}