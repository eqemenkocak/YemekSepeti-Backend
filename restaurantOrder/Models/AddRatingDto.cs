namespace restaurantOrder.Models
{
    public class AddRatingDto
    {
        public int ProductId { get; set; } // Hangi yemeğe puan veriliyor?
        public int OrderId { get; set; }   // Hangi siparişteydi?
        public int Score { get; set; }     // Kaç puan? (1-5 arası)
    }
}