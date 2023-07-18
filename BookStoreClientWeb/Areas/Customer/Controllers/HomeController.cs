using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BookStoreClientWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private string ApiUrl = "";
        private readonly ILogger<HomeController> _logger;
        private HttpResponseMessage _response;
        public HomeController(ILogger<HomeController> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiUrl = "https://localhost:7275/api/Product";
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _response = await _httpClient.GetAsync(ApiUrl);
            if (_response.IsSuccessStatusCode)
            {
                var productResponse = await _response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                IEnumerable<Product> products = JsonSerializer.Deserialize<IEnumerable<Product>>(productResponse,options);
                return View(products);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int productId)
        {
            _response = await _httpClient.GetAsync($"https://localhost:7275/api/Product/id?id={productId}");
            if (_response.IsSuccessStatusCode)
            {
                var productResponse = await _response.Content.ReadAsStringAsync();
                var option = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Product product = JsonSerializer.Deserialize<Product>(productResponse, option);
               
                return View(product);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}