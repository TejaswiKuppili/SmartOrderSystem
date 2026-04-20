using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Data;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new order to the DbContext without saving changes
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        /// <summary>
        /// Retrieves an order by ID including its related items from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
