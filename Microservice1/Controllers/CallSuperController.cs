using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CallSuperController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CallSuperController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var client = _httpClientFactory.CreateClient("supermicroservice");
            try
            {
                // This should fail
                var result = await client.GetStringAsync("weatherforecast");

                return $"Result from Microservice1 calling Supermicroservice: {result}";
            }
            catch (Exception e)
            {
                return $"Microservice1 failed to call Supermicroservice: {e.ToString()}";
            }
        }
    }
}
