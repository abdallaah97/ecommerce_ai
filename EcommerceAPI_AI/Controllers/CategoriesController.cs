using EcommerceAPI_AI.Context;
using EcommerceAPI_AI.Dtos;
using EcommerceAPI_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CategoriesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateCategory")]
        public ActionResult<CategoryOutput> CreateCategory([FromBody] CreateCategoryInput input)
        {
            var category = new Category
            {
                Name = input.Name,
                Description = input.Description
            };

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();

            return Ok(new CategoryOutput
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0
            });
        }

        [HttpGet("GetCategories")]
        public ActionResult<List<CategoryOutput>> GetCategories()
        {
            var categories = _dbContext.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryOutput
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count
                })
                .ToList();

            return Ok(categories);
        }

        [HttpGet("GetCategory/{id}")]
        public ActionResult<CategoryOutput> GetCategory(int id)
        {
            var category = _dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            return Ok(new CategoryOutput
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.Products.Count
            });
        }

        [HttpDelete("DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            if (category.Products.Any())
            {
                return BadRequest("Cannot delete category because it contains associated products.");
            }

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            return Ok(new { Message = $"Category with ID {id} deleted successfully." });
        }
    }
}

