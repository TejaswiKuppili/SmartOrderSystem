namespace BuildingBlocks.Events
{
    public class PaymentSuccessEvent
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = "Success";
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
