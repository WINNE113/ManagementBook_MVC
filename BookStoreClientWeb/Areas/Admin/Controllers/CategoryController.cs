
using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookStoreClientWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiUrl = "";
        public CategoryController()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7275/api/Category";
        }

        public async Task<IActionResult> Index()
        {
            string token = HttpContext.Session.GetString("token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);
            var category = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Category> categories = JsonSerializer.Deserialize<List<Category>>(category,options);
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                var JsonModel = JsonSerializer.Serialize(obj);
                var content = new StringContent(JsonModel, Encoding.UTF8, "application/json");
                string token = HttpContext.Session.GetString("token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsync(ApiUrl,content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Category created fail!";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            string token = HttpContext.Session.GetString("token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7275/api/Category?Filter = Id eq {id}");
            var categoryResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            if(categoryResponse != null)
            {
                List<Category> ListCategory = JsonSerializer.Deserialize<List<Category>>(categoryResponse, options);
                Category category = ListCategory[0];
                return (category != null) ? View(category) : RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                var JsonModel = JsonSerializer.Serialize(obj);
                var content = new StringContent(JsonModel, Encoding.UTF8, "application/json");
                string token = HttpContext.Session.GetString("token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PutAsync($"https://localhost:7275/api/Category?id={obj.Id}",content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category updated successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Category updated fail!";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            string token = HttpContext.Session.GetString("token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7275/api/Category?Filter = Id eq {id}");
            var categoryResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            if (categoryResponse != null)
            {
                List<Category> ListCategory = JsonSerializer.Deserialize<List<Category>>(categoryResponse, options);
                Category categoryFromDb = ListCategory[0];
                return (categoryFromDb != null) ? View(categoryFromDb) : RedirectToAction("Index");
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Category obj)
        {
            if (ModelState.IsValid)
            {
                string token = HttpContext.Session.GetString("token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.DeleteAsync($"https://localhost:7275/api/Category/id?id={obj.Id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Category delete successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Category delete fail!";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
    }
}
