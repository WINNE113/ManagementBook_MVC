using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookStoreApi.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; // To get image
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult GetProduts()
        {
            return Ok(_unitOfWork.Product.GetAll(includeProperties: "Category").ToList());
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpGet("id")]
        public IActionResult Detail(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id ,includeProperties : "Category");
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product request)
        {
            try
            {
                _unitOfWork.Product.Add(request);
                _unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpDelete("id")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                Product product = _unitOfWork.Product.Get(u => u.Id == id);
                if (product != null)
                {
                    _unitOfWork.Product.Remove(product);
                    _unitOfWork.Save();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var slnPath = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).ToString();
            //var oldImagePath =
            //             Path.Combine(_webHostEnvironment.WebRootPath,
            //             productToBeDeleted.ImageUrl.TrimStart('\\'));
            var PathWeb = Path.Combine(slnPath, "BookStoreClientWeb", "wwwroot");   //D:\WorkSpace\.Net\ManagementBook\BookStoreClientWeb\wwwroot
            var oldImagePath = Path.Combine(PathWeb, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product request)
        {
            try
            {
                _unitOfWork.Product.Update(request);
                _unitOfWork.Save();
                return Ok(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
