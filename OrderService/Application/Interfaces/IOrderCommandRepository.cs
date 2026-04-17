using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IOrderCommandRepository
    {
        Task<Guid> CreateOrderWithTransactionAsync(Order order);
    }
}
