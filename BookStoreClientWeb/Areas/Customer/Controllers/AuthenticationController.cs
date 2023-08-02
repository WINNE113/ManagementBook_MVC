using BookStore.Models.Authentication.Login;
using BookStore.Models.Authentication.SignUp;
using BookStore.Models.ModelsToRequest;
using BookStore.Models.ViewModels;
using BookStore.Unitily.SD;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookStoreClientWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private HttpClient _httpClient;
        private string ApiUrl = "";
        private HttpResponseMessage _response;
        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ApiUrl = "https://localhost:7275/api/Authentication";
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
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
                var content = new StringContent(JsonModel, Encoding.UTF8, "application/json");
                _response = await _httpClient.PostAsync("https://localhost:7275/api/Authentication/login", content);
                if (_response.IsSuccessStatusCode)
                {
                    var userDb = await _userManager.FindByNameAsync(login.UserName);

                    if (userDb.TwoFactorEnabled == true)
                    {
                        TempData["UserName"] = userDb.UserName;
                        await _signInManager.SignOutAsync();
                        await _signInManager.PasswordSignInAsync(userDb, login.Password, false, true);
                        return RedirectToAction("LoginTwoFactor");
                    }

                    var response = await _response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<JsonElement>(response);
                    string token = responseObject.GetProperty("token").GetString();

                    HttpContext.Session.SetString("token", token);

                    var user = new IdentityUser()
                    {
                        UserName = login.UserName,
                    };

                    if (userDb != null)
                    {
                        var roleDb = await _userManager.GetRolesAsync(userDb);

                        HttpContext.Session.SetString("role", roleDb[0]); // TODO
                    }

                    await _signInManager.SignInAsync(user, false);

                }

                return RedirectToAction("Index", "Home");
            }
            TempData["error"] = "Login was fail!";

            return View();
        }
        [HttpGet]
        public IActionResult LoginTwoFactor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginTwoFactor(string OTPCode)
        {
            if (ModelState.IsValid)
            {
                var userName = TempData["UserName"];
                var signIn = await _signInManager.TwoFactorSignInAsync("Email", OTPCode, false, false);
                if (signIn.Succeeded)
                {
                    _response = await _httpClient.GetAsync($"https://localhost:7275/api/Authentication/Token?UserName={userName}");
                    if (_response.IsSuccessStatusCode)
                    {
                        var response = await _response.Content.ReadAsStringAsync();
                        var responseObject = JsonSerializer.Deserialize<JsonElement>(response);
                        string token = responseObject.GetProperty("token").GetString();

                        HttpContext.Session.SetString("token", token);

                        var userDb = await _userManager.FindByNameAsync(userName.ToString());
                        var user = new IdentityUser()
                        {
                            UserName = userDb.UserName,
                            Email = userDb.Email,
                        };
                        if (userDb != null)
                        {
                            var roleDb = await _userManager.GetRolesAsync(userDb);

                            HttpContext.Session.SetString("role", roleDb[0]); // TODO
                        }

                        await _signInManager.SignInAsync(user, false);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["error"] = "Something is wrong!";
                    }
                }
                else
                {
                    TempData["error"] = "OTP Code is wrong!";
                }
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
                var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
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
                    return RedirectToAction("Index", "Home");
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
            HttpContext.Session.Remove("role");
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
