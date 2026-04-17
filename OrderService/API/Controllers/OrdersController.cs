using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrders(string userId,
    [FromServices] IOrderQueryRepository queryRepo)
        {
            var orders = await queryRepo.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var id = await _mediator.Send(new CreateOrderCommand(dto));
            return Ok(id);
        }
    }
}
