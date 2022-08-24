using Microsoft.AspNetCore.Mvc;

namespace Api1.Controllers;

[ApiController]
[Route("api/consignments")]
public class ConsignmentController: ControllerBase
{
    private readonly ILogger<ConsignmentController> _logger;

    public ConsignmentController(ILogger<ConsignmentController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index(Consignment consignment)
    {
        _logger.LogInformation($"Requesting ContactAddress: {consignment.ContactAddress}, WarehouseAddress: {consignment.WarehouseAddress}");
        
        var random = new Random();
        var randomDelay = random.Next(1, 6);
        // 20% of returning 500
        if (randomDelay == 5)
        {
            return StatusCode(500);
        }
        // Random delay from 1-5 seconds
        await Task.Delay(randomDelay * 1000);
        
        return Ok(new
        {
            Total = random.NextDouble() * 100
        });
    }
}