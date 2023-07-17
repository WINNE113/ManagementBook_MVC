
using BookStore.DataAccess.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreClientWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            List<Category> categories = _dbContext.Categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(obj);
                _dbContext.SaveChanges();
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");

            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null && id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if(categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(obj);
                _dbContext.Update(obj);
                _dbContext.SaveChanges();
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction("Index");

            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Remove(obj); 
                _dbContext.SaveChanges();
                TempData["success"] = "Category deleted successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
