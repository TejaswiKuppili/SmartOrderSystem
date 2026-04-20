namespace OrderService.Domain.Entities
{
    /// <summary>
    /// Represents an individual item within an order, including product details, quantity, and price
    /// </summary>
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
