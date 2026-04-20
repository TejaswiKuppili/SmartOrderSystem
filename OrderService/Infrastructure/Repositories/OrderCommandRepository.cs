using Dapper;
using Microsoft.Data.SqlClient;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderCommandRepository : IOrderCommandRepository
    {
        private readonly IConfiguration _configuration;

        public OrderCommandRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Handles order persistence using raw SQL with a transaction, inserting the order and its items
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<Guid> CreateOrderWithTransactionAsync(Order order)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var orderQuery = @"
                INSERT INTO Orders (Id, UserId, TotalAmount, CreatedAt)
                VALUES (@Id, @UserId, @TotalAmount, @CreatedAt)";

                await connection.ExecuteAsync(orderQuery, new
                {
                    order.Id,
                    order.UserId,
                    order.TotalAmount,
                    order.CreatedAt
                }, transaction);

                var itemQuery = @"
                INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, Price)
                VALUES (@Id, @OrderId, @ProductId, @Quantity, @Price)";

                foreach (var item in order.Items)
                {
                    await connection.ExecuteAsync(itemQuery, new
                    {
                        item.Id,
                        OrderId = order.Id,
                        item.ProductId,
                        item.Quantity,
                        item.Price
                    }, transaction);
                }

                transaction.Commit();
                return order.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
