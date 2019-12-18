using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UITester.Models;

namespace UITester.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> CallApiWithAuthenticatedUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var content = await client.GetStringAsync("https://localhost:44334/api/v1/supermicroservice/weatherforecast");

            return new ContentResult() { Content = content };
        }

        public async Task<IActionResult> CallMicroservice1()
        {
            var client = _httpClientFactory.CreateClient("microservice1");

            var response = await client.GetAsync("weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                return new ContentResult() { Content = response.StatusCode.ToString() };
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new ContentResult() { Content = content };
            }
        }


        public async Task<IActionResult> CallMicroservice2()
        {
            var client = _httpClientFactory.CreateClient("microservice2");

            var response = await client.GetAsync("weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                return new ContentResult() { Content = response.StatusCode.ToString() };
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new ContentResult() { Content = content };
            }
        }

        public async Task<IActionResult> CallSuperMicroservice()
        {
            // This should fail. Configure identityservice to add the required scope "supermicroservice" to the mysupermicroserviceclient
            var client = _httpClientFactory.CreateClient("supermicroservice");

            var response = await client.GetAsync("weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                return new ContentResult() { Content = response.StatusCode.ToString() };
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new ContentResult() { Content = content };
            }
        }

        public async Task<string> TestMicroservice1()
        {
            var client = _httpClientFactory.CreateClient("microservice1");

            var response = await client.GetStringAsync("test");
            return response;
        }

        public async Task<string> TestMicroservice2()
        {
            var client = _httpClientFactory.CreateClient("microservice2");

            var response = await client.GetStringAsync("test");
            return response;
        }

        public async Task<string> TestSuperMicroservice()
        {
            // This should fail. Configure identityservice to add the required scope "supermicroservice" to the mysupermicroserviceclient
            var client = _httpClientFactory.CreateClient("supermicroservice");

            var response = await client.GetStringAsync("test");
            return response;
        }

        public async Task<string> TestSuperMicroservice2()
        {
            var client = _httpClientFactory.CreateClient("microservice1");

            var response = await client.GetStringAsync("callsuper");
            return response;
        }

        public async Task<string> TestSuperMicroservice3()
        {
            var client = _httpClientFactory.CreateClient("microservice2");

            var response = await client.GetStringAsync("callsuper");
            return response;
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    //var client = new HttpClient();
    //var disco = await client.GetDiscoveryDocumentAsync("https://localhost:44334/idservice");
    //if (disco.IsError)
    //{
    //    return new ContentResult() { Content = disco.Error };
    //}

    //// request token
    //var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    //{
    //    Address = disco.TokenEndpoint,

    //    ClientId = "mymvcclient",
    //    ClientSecret = "secret",
    //    Scope = "microservice1"
    //});

    //if (tokenResponse.IsError)
    //{
    //    return new ContentResult() { Content = tokenResponse.Error };
    //}

    //// call api
    //var apiClient = new HttpClient();
    //apiClient.SetBearerToken(tokenResponse.AccessToken);

    //var response = await apiClient.GetAsync("https://localhost:44334/api/v1/microservice1/weatherforecast");
    //if (!response.IsSuccessStatusCode)
    //{
    //    return new ContentResult() { Content = response.StatusCode.ToString() };
    //}
    //else
    //{
    //    var content = await response.Content.ReadAsStringAsync();
    //    return new ContentResult() { Content = content };
    //}
}
