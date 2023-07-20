using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace BookStoreApi.Controllers.Customer
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductCustomerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductCustomerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [EnableQuery]
        [HttpGet]
        public IActionResult GetProduts()
        {
            return Ok(_unitOfWork.Product.GetAll(includeProperties: "Category").ToList());
        }
        [HttpGet("id")]
        public IActionResult Detail(int id)
        {
            Product product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category");
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
