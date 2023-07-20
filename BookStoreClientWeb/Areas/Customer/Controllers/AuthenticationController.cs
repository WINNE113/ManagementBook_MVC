﻿using BookStore.Models.Authentication.Login;
using BookStore.Models.Authentication.SignUp;
using BookStore.Models.ViewModels;
using BookStore.Unitily.SD;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookStoreClientWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private HttpClient _httpClient;
        private string ApiUrl = "";
        private HttpResponseMessage _response;
        private const string ROLE_CUSTOMER = "Customer";

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiUrl = "https://localhost:7275/api/Authentication";
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var JsonModel = JsonSerializer.Serialize(login);
                var content = new StringContent(JsonModel,Encoding.UTF8,"application/json");
                _response = await _httpClient.PostAsync("https://localhost:7275/api/Authentication/login", content);
                if (_response.IsSuccessStatusCode)
                {
                    var response = await _response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<JsonElement>(response);
                    string token = responseObject.GetProperty("token").GetString();
                    HttpContext.Session.SetString("token", token);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var user = new IdentityUser() {
                        UserName = login.UserName,
                    };
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                TempData["error"] = "Login was fail!";
            }
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser registerUser)
        {
            if (ModelState.IsValid)
            {
                var jsonModel = JsonSerializer.Serialize(registerUser);
                var content = new StringContent(jsonModel,Encoding.UTF8,"application/json");    
                _response = await _httpClient.PostAsync($"https://localhost:7275/api/Authentication/Role?Role={SD.Role_Customer}", content);
                if (_response.IsSuccessStatusCode)
                {
                    var user = new IdentityUser()
                    {
                        UserName = registerUser.UserName,
                        Email = registerUser.Email,
                    };
                    TempData["success"] = "Register was successfully!";
                     await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index","Home");
                }
                TempData["error"] = "Register was fail!";
            }
            ModelState.AddModelError("", "UserName or Password incorrect");
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");
            _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");    
        }
    }
}