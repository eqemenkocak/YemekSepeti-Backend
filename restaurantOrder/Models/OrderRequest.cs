namespace restaurantOrder.Models
{
    // React'ten bize gelecek olan paket bu
    public class OrderRequest
    {
        public int UserId { get; set; } // Kim sipariş verdi?
        public int RestaurantId { get; set; } // Hangi restoran?
        public List<int> ProductIds { get; set; } // Hangi ürünleri aldı? (ID listesi)
        public decimal TotalAmount { get; set; } // Kaç para tuttu?
    }
}