namespace restaurantOrder.Models
{
    public class OrderRequest
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public List<int> ProductIds { get; set; }
        public decimal TotalAmount { get; set; }

        // 👇 YENİ EKLENEN ALAN
        public string PaymentMethod { get; set; }
    }
}