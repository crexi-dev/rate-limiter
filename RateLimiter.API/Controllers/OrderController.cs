using Microsoft.AspNetCore.Mvc;
using RateLimiter.Models;
using RateLimiter.Repositories;

namespace RateLimiter.API.Controllers
{
    [Route("products")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        public OrderController(IOrderRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [Limit.LimitRequests(MaxRequests = 2, TimeWindow = 5)]
        public IActionResult GetAllProducts()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProduct(Guid id)
        {
            var order = _repo.GetById(id);
            return order is not null ? Ok(order) : NotFound();
        }
    }
}
