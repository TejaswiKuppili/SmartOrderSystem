using Dapper;
using Microsoft.Data.SqlClient;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using System.Data;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderQueryRepository : IOrderQueryRepository
    {
        private readonly IConfiguration _configuration;

        public OrderQueryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        /// <summary>
        /// Retrieves orders for a user using a SQL join and maps results into order aggregates with their items
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
            var query = @"
            SELECT o.Id, o.UserId, o.TotalAmount, o.CreatedAt,
                   i.Id, i.ProductId, i.Quantity, i.Price, i.OrderId
            FROM Orders o
            LEFT JOIN OrderItems i ON o.Id = i.OrderId
            WHERE o.UserId = @UserId";

            using var connection = CreateConnection();

            var orderDictionary = new Dictionary<Guid, Order>();

            var result = await connection.QueryAsync<Order, OrderItem, Order>(
                query,
                (order, item) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var existingOrder))
                    {
                        existingOrder = order;
                        existingOrder.Items = new List<OrderItem>();
                        orderDictionary.Add(order.Id, existingOrder);
                    }

                    if (item != null)
                        existingOrder.Items.Add(item);

                    return existingOrder;
                },
                new { UserId = userId },
                splitOn: "Id"
            );

            return orderDictionary.Values;
        }
    }
}
