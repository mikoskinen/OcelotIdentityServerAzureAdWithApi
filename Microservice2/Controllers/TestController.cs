using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var client = _httpClientFactory.CreateClient("microservice1");
            try
            {
                var result = await client.GetStringAsync("weatherforecast");

                return $"Result from Microservice2 calling Microservice1: {result}";
            }
            catch (Exception e)
            {
                return $"Microservice2 failed to call Microservice1: {e.ToString()}";
            }
        }
    }
}
