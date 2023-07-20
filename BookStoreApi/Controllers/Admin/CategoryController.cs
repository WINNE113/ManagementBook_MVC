using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Unitily.SD;
using BookStoreApi.ModelsRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OData.Query;

namespace BookStoreApi.Controllers.Admin
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult GetCategories()
        {
            try
            {
                var ListCategory = _unitOfWork.Category.GetAll().ToList();
                return Ok(ListCategory);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryRequest request)
        {
            try
            {
                Category category = new Category
                {
                    Name = request.Name,
                    DisplayOrder = request.DisplayOrder
                };
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete("id")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                Category category = _unitOfWork.Category.Get(u => u.Id == id);
                if (category != null)
                {
                    _unitOfWork.Category.Remove(category);
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
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPut]
        public IActionResult UpdateCategory([FromBody] CategoryRequest request, int id)
        {
            try
            {
                Category _category = _unitOfWork.Category.Get(u => u.Id == id);
                if (_category != null)
                {
                    _category.Name = request.Name;
                    _category.DisplayOrder = request.DisplayOrder;
                    _unitOfWork.Category.Update(_category);
                    _unitOfWork.Save();
                    return Ok(_category);
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
    }
}
