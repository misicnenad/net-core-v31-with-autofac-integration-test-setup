using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NetCoreV31.Interfaces;
using NetCoreV31.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreV31.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IRequestValidationService _validationService;

        public WeatherForecastController(IRequestValidationService validationService)
        {
            _validationService = validationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<WeatherForecast>> Get()
        {
            if (!_validationService.RequestCanBeProcessed())
            {
                return BadRequest();
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
