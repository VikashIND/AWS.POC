namespace OrderService.Models
{
    public class OrderCreatedNotification
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public OrderCreatedNotification(int orderId, int customerId, List<ProductDetail> productDetails)
        {
            OrderId = orderId;
            CustomerId = customerId;
            CreatedAt = DateTime.UtcNow;
            ProductDetails = productDetails ?? new List<ProductDetail>();
        }
    }
}
