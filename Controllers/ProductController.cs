using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Pagination.Models;
using Serilog;

namespace Pagination.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<ProductController> _logger;

		public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet]
		public ActionResult<IEnumerable<Product>> Get(int page = 1, int pageSize = 10)
		{
			try
			{
				var totalCount = _context.Products.Count();
				var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
				var products = _context.Products
					.OrderByDescending(p => p.Id)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();

				_logger.LogInformation($"Retrieved products for page {page} with {pageSize} items per page.");

				return Ok(new { TotalCount = totalCount, TotalPages = totalPages, Products = products });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while processing the request.");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPost]
		public ActionResult<Product> Post(Product product)
		{
			try
			{
				_context.Products.Add(product);
				_context.SaveChanges();
				_logger.LogInformation($"Added new product with ID: {product.Id}");
				return Ok(product);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while adding the product.");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpPut("{id}")]
		public ActionResult<Product> Put(int id, Product product)
		{
			try
			{
				var existingProduct = _context.Products.Find(id);
				if (existingProduct == null)
				{
					return NotFound();
				}

				existingProduct.Name = product.Name;
				existingProduct.LastName = product.LastName;

				_context.SaveChanges();
				_logger.LogInformation($"Updated product with ID: {id}");
				return Ok(existingProduct);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while updating the product with ID: {id}");
				return StatusCode(500, "Internal server error");
			}
		}

		[HttpDelete("{id}")]
		public ActionResult<Product> Delete(int id)
		{
			try
			{
				var product = _context.Products.Find(id);
				if (product == null)
				{
					return NotFound();
				}

				_context.Products.Remove(product);
				_context.SaveChanges();
				_logger.LogInformation($"Deleted product with ID: {id}");
				return Ok(product);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"An error occurred while deleting the product with ID: {id}");
				return StatusCode(500, "Internal server error");
			}
		}
	}
	}
