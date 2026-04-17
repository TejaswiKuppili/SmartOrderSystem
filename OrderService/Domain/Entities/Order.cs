namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}
