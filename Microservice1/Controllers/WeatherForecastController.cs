using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            var result = $"Hello from Microservice 1. Claims: {string.Join(", ", User?.Claims?.Select(x => x.Type + ":" + x.Value))}";

            return result;
        }
    }
}
