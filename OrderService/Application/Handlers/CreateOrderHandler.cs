using BuildingBlocks.EventBus;
using BuildingBlocks.Events;
using MediatR;
using OrderService.Application.Commands;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Handlers
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderCommandRepository CommandRepo;
        private readonly IEventBus EventBus;

        public CreateOrderHandler(IOrderCommandRepository commandRepo, IEventBus eventBus)
        {
            CommandRepo = commandRepo;
            EventBus = eventBus;
        }

        /// <summary>
        /// Handles the CreateOrderCommand by building the order, saving it transactionally, and publishing an OrderCreated event
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.OrderDto.UserId,
                CreatedAt = DateTime.UtcNow,
                Items = request.OrderDto.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            order.TotalAmount = order.Items.Sum(x => x.Price * x.Quantity);

            var orderId = await CommandRepo.CreateOrderWithTransactionAsync(order);

            // Publish event
            EventBus.Publish(new OrderCreatedEvent
            {
                OrderId = orderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount
            });

            return orderId;
        }
    }
}
