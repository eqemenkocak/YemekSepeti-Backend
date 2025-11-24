namespace restaurantOrder.Models
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        // Şifre değiştirme opsiyonel olsun (Boş gelirse değiştirme)
        public string? Password { get; set; }
    }
}