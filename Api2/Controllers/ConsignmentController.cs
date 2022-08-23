using Microsoft.AspNetCore.Mvc;

namespace Api1.Controllers;

[ApiController]
[Route("api/consignments")]
public class ConsignmentController: ControllerBase
{
    public async Task<IActionResult> Index(Consignment consignment)
    {
        Console.WriteLine($"Requesting consignee: {consignment.Consignee}, destination: {consignment.Consigner}");
        
        var random = new Random();
        var randomDelay = random.Next(1, 11);
        // Random delay from 1-10 seconds
        await Task.Delay(randomDelay * 1000);
        return Ok(new
        {
            Amount = random.NextDouble() * 100
        });
    }
}