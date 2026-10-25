using EcommerceAPI_AI.Context;
using EcommerceAPI_AI.Dtos;
using EcommerceAPI_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TagsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateTag")]
        public ActionResult<TagOutput> CreateTag([FromBody] CreateTagInput input)
        {
            var tag = new Tag
            {
                Name = input.Name
            };

            _dbContext.Tags.Add(tag);
            _dbContext.SaveChanges();

            return Ok(new TagOutput
            {
                Id = tag.Id,
                Name = tag.Name
            });
        }

        [HttpGet("GetTags")]
        public ActionResult<List<TagOutput>> GetTags()
        {
            var tags = _dbContext.Tags
                .Select(t => new TagOutput
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToList();

            return Ok(tags);
        }

        [HttpDelete("DeleteTag/{id}")]
        public IActionResult DeleteTag(int id)
        {
            var tag = _dbContext.Tags.FirstOrDefault(t => t.Id == id);
            if (tag == null)
            {
                return NotFound($"Tag with ID {id} not found.");
            }

            _dbContext.Tags.Remove(tag);
            _dbContext.SaveChanges();

            return Ok(new { Message = $"Tag with ID {id} deleted successfully." });
        }
    }
}

