using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStoreClientWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
