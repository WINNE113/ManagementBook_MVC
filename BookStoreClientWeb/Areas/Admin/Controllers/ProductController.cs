using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookStoreClientWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string ApiUrl = "";
       // private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; // To get image
        private HttpResponseMessage _response;
        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            ApiUrl = "https://localhost:7275/api/Product";
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            _response = await _httpClient.GetAsync("https://localhost:7275/api/Category");
            var productResponse = await _response.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Product> objProductList = JsonSerializer.Deserialize<List<Product>>(productResponse, option);
            // List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }

        public async Task<IActionResult> UpSert(int? id)
        {
            _response = await _httpClient.GetAsync("https://localhost:7275/api/Category");
            var categoryRespone = await _response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Category> categories = JsonSerializer.Deserialize<List<Category>>(categoryRespone, options);
            IEnumerable<SelectListItem>? selectListItems = categories.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString(),
            });
            ProductVM productVM = new()
            {
                CategoryList = selectListItems.ToArray(),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                _response = await _httpClient.GetAsync($"https://localhost:7275/api/Product?Filter = id eq {id}");
                var productRespone = await _response.Content.ReadAsStringAsync();
                List<Product> products = JsonSerializer.Deserialize<List<Product>>(productRespone, options);
                if (products != null)
                {
                    productVM.Product = products[0];
                }
                //update
                return View(productVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpSert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\\" + fileName;
                }
                // Check if id == 0 will create 
                if (productVM.Product.Id == 0)
                {
                    var JsonModel = JsonSerializer.Serialize(productVM.Product);
                    var content = new StringContent(JsonModel, Encoding.UTF8, "application/json");
                    _response = await _httpClient.PostAsync(ApiUrl, content);
                    if (_response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Product created successfully";
                    }
                    else
                    {
                        TempData["error"] = "Product created fail!";
                    }
                }
                else
                {
                    var JsonModel = JsonSerializer.Serialize(productVM.Product);
                    var content = new StringContent(JsonModel, Encoding.UTF8, "application/json");
                    _response = await _httpClient.PutAsync(ApiUrl, content);
                    if (_response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Product update successfully";
                    }
                    else
                    {
                        TempData["error"] = "Product update fail!";
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                _response = await _httpClient.GetAsync("https://localhost:7275/api/Category");
                var categoryRespone = await _response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                List<Category> categories = JsonSerializer.Deserialize<List<Category>>(categoryRespone, options);
                IEnumerable<SelectListItem>? selectListItems = categories.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                ProductVM product = new()
                {
                    CategoryList = selectListItems.ToArray(),
                    Product = new Product()
                };
                return View(product);
            }
        }
    } 
}
