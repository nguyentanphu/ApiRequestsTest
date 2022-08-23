﻿using Api1;
using Microsoft.AspNetCore.Mvc;

namespace Api3.Controllers;

[ApiController]
[Route("api/consignments")]
public class ConsignmentController: ControllerBase
{
    public async Task<IActionResult> Index(PackageInput packageInput)
    {
        Console.WriteLine($"Requesting source: {packageInput.Source}, destination: {packageInput.Destination}");
        
        var random = new Random();
        var randomDelay = random.Next(1, 11);
        // Random delay from 1-10 seconds
        await Task.Delay(randomDelay * 1000);
        return Ok(new PackageQuote
        {
            Quote = random.NextDouble() * 100
        });
    }
}