using EcommerceAPI_AI.Context;
using EcommerceAPI_AI.Dtos;
using EcommerceAPI_AI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI_AI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateProduct")]
        public ActionResult<ProductOutput> CreateProduct([FromBody] CreateProductInput input)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == input.CategoryId);
            if (category == null)
            {
                //400
                return BadRequest($"Category with ID {input.CategoryId} does not exist.");
            }

            PhysicalProductProfile? physical = null;
            DigitalProductProfile? digital = null;

            if (input.Type == ProductType.Physical)
            {
                if (input.Physical == null)
                {
                    return BadRequest("Physical product details must be provided.");
                }

                physical = new PhysicalProductProfile
                {
                    WeightKg = input.Physical.WeightKg,
                    Dimensions = input.Physical.Dimensions,
                    WarehouseLocation = input.Physical.WarehouseLocation
                };

                _dbContext.PhysicalProductProfiles.Add(physical);
                _dbContext.SaveChanges();
            }
            else if (input.Type == ProductType.Digital)
            {
                if (input.Digital == null)
                {
                    return BadRequest("Digital product details must be provided.");
                }

                digital = new DigitalProductProfile
                {
                    DownloadUrl = input.Digital.DownloadUrl,
                    FileSizeMB = input.Digital.FileSizeMB,
                    FileFormat = input.Digital.FileFormat
                };

                _dbContext.DigitalProductProfiles.Add(digital);
                _dbContext.SaveChanges();
            }

            var product = new Product
            {
                Name = input.Name,
                Sku = input.Sku,
                Price = input.Price,
                CreatedAt = DateTime.UtcNow,
                Type = input.Type,
                CategoryId = input.CategoryId,
                PhysicalProfileId = physical?.Id,
                DigitalProfileId = digital?.Id
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();

            if (input.TagIds != null && input.TagIds.Any())
            {
                foreach (var tagId in input.TagIds)
                {
                    var tagExists = _dbContext.Tags.Any(t => t.Id == tagId);
                    if (tagExists)
                    {
                        var productTag = new ProductTag
                        {
                            ProductId = product.Id,
                            TagId = tagId,
                            AssignedAt = DateTime.UtcNow
                        };
                        _dbContext.ProductTags.Add(productTag);
                    }
                }
                _dbContext.SaveChanges();
            }

            return GetProduct(product.Id);
        }

        [HttpPut("UpdateProduct/{id}")]
        public ActionResult<ProductOutput> UpdateProduct(int id, [FromBody] UpdateProductInput input)
        {
            var product = _dbContext.Products
                .Include(p => p.PhysicalProfile)
                .Include(p => p.DigitalProfile)
                .Include(p => p.ProductTags)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                // 404
                return NotFound($"Product with ID {id} not found.");
            }

            var categoryExists = _dbContext.Categories.Any(c => c.Id == input.CategoryId);
            if (!categoryExists)
            {
                //400
                return BadRequest($"Category with ID {input.CategoryId} does not exist.");
            }

            product.Name = input.Name;
            product.Sku = input.Sku;
            product.Price = input.Price;
            product.CategoryId = input.CategoryId;

            if (product.Type == ProductType.Physical && input.Physical != null && product.PhysicalProfile != null)
            {
                product.PhysicalProfile.WeightKg = input.Physical.WeightKg;
                product.PhysicalProfile.Dimensions = input.Physical.Dimensions;
                product.PhysicalProfile.WarehouseLocation = input.Physical.WarehouseLocation;
                _dbContext.PhysicalProductProfiles.Update(product.PhysicalProfile);
            }
            else if (product.Type == ProductType.Digital && input.Digital != null && product.DigitalProfile != null)
            {
                product.DigitalProfile.DownloadUrl = input.Digital.DownloadUrl;
                product.DigitalProfile.FileSizeMB = input.Digital.FileSizeMB;
                product.DigitalProfile.FileFormat = input.Digital.FileFormat;
                _dbContext.DigitalProductProfiles.Update(product.DigitalProfile);
            }

            if (input.TagIds != null)
            {
                _dbContext.ProductTags.RemoveRange(product.ProductTags);

                foreach (var tagId in input.TagIds)
                {
                    if (_dbContext.Tags.Any(t => t.Id == tagId))
                    {
                        _dbContext.ProductTags.Add(new ProductTag
                        {
                            ProductId = product.Id,
                            TagId = tagId,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return GetProduct(product.Id);
        }

        [HttpPatch("UpdateProductPrice/{id}")]
        public ActionResult<ProductOutput> UpdateProductPrice(int id, [FromBody] UpdateProductPriceInput input)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            product.Price = input.Price;
            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            return GetProduct(product.Id);
        }

        [HttpGet("GetProduct/{id}")]
        public ActionResult<ProductOutput> GetProduct(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.PhysicalProfile)
                .Include(p => p.DigitalProfile)
                .Include(p => p.Reviews)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            var result = MapToOutput(product);

            //200
            return Ok(result);
        }

        [HttpGet("GetProducts")]
        public ActionResult<List<ProductOutput>> GetProducts()
        {
            var products = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.PhysicalProfile)
                .Include(p => p.DigitalProfile)
                .Include(p => p.Reviews)
                .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                .ToList();

            var result = products.Select(MapToOutput).ToList();
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.PhysicalProfile)
                .Include(p => p.DigitalProfile)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            if (product.PhysicalProfile != null)
            {
                _dbContext.PhysicalProductProfiles.Remove(product.PhysicalProfile);
            }
            if (product.DigitalProfile != null)
            {
                _dbContext.DigitalProductProfiles.Remove(product.DigitalProfile);
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Ok(new { Message = $"Product with ID {id} and its associated relations were deleted successfully." });
        }

        [HttpPost("AddReview/{productId}")]
        public ActionResult<ProductOutput> AddReview(int productId, [FromBody] CreateProductReviewInput input)
        {
            var productExists = _dbContext.Products.Any(p => p.Id == productId);
            if (!productExists)
            {
                return NotFound($"Product with ID {productId} not found.");
            }

            var review = new ProductReview
            {
                ProductId = productId,
                ReviewerName = input.ReviewerName,
                Rating = input.Rating,
                Comment = input.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.ProductReviews.Add(review);
            _dbContext.SaveChanges();

            return GetProduct(productId);
        }

        [HttpPost("AssignTags/{productId}")]
        public ActionResult<ProductOutput> AssignTags(int productId, [FromBody] AssignTagsInput input)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductTags)
                .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound($"Product with ID {productId} not found.");
            }

            foreach (var tagId in input.TagIds)
            {
                var alreadyAssigned = product.ProductTags.Any(pt => pt.TagId == tagId);
                var tagExists = _dbContext.Tags.Any(t => t.Id == tagId);

                if (!alreadyAssigned && tagExists)
                {
                    _dbContext.ProductTags.Add(new ProductTag
                    {
                        ProductId = productId,
                        TagId = tagId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            _dbContext.SaveChanges();
            return GetProduct(productId);
        }

        private static ProductOutput MapToOutput(Product product)
        {
            return new ProductOutput
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                Type = product.Type,
                Category = product.Category != null ? new ProductCategoryOutput
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                } : null,
                Physical = product.Type == ProductType.Physical && product.PhysicalProfile != null ? new PhysicalProfileOutput
                {
                    Id = product.PhysicalProfile.Id,
                    WeightKg = product.PhysicalProfile.WeightKg,
                    Dimensions = product.PhysicalProfile.Dimensions,
                    WarehouseLocation = product.PhysicalProfile.WarehouseLocation
                } : null,
                Digital = product.Type == ProductType.Digital && product.DigitalProfile != null ? new DigitalProfileOutput
                {
                    Id = product.DigitalProfile.Id,
                    DownloadUrl = product.DigitalProfile.DownloadUrl,
                    FileSizeMB = product.DigitalProfile.FileSizeMB,
                    FileFormat = product.DigitalProfile.FileFormat
                } : null,
                Reviews = product.Reviews.Select(r => new ProductReviewOutput
                {
                    Id = r.Id,
                    ReviewerName = r.ReviewerName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToList(),
                Tags = product.ProductTags.Select(pt => new TagOutput
                {
                    Id = pt.Tag.Id,
                    Name = pt.Tag.Name
                }).ToList()
            };
        }
    }
}
