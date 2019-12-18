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
            var client2 = _httpClientFactory.CreateClient("microservice2");
            try
            {
                var result = await client.GetStringAsync("weatherforecast");

                var result2 = await client2.GetStringAsync("weatherforecast");

                return $"Result from Supermicroservice calling Microservice1: {result}. Result from Supermicroservice calling Microservice2: {result2}";
            }
            catch (Exception e)
            {
                return $"SuperMicroservice failed to call Microservice2 or Microservice1: {e.ToString()}";
            }
        }
    }
}
