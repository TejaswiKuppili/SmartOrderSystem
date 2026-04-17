using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IOrderQueryRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserId(string userId);
    }
}
