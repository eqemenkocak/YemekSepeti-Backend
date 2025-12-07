namespace restaurantOrder.Models
{
    public class StatsDto
    {
        public decimal Revenue { get; set; }    // Ciro
        public int OrderCount { get; set; }     // Sipariş Sayısı
        public int ProductCount { get; set; }   // Ürün Sayısı
    }
}