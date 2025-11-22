using Microsoft.AspNetCore.Mvc;
using restaurantOrder.Models;

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RestaurantOrderDbContext _context;

        public AuthController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Kullanıcıyı bul
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized("Hatalı E-posta veya Şifre!");
            }

            // 2. Eğer Restoran Sahibiyse, dükkanını bul
            int? restaurantId = null;
            if (user.Role == "RestaurantOwner")
            {
                var restaurant = _context.Restaurants.FirstOrDefault(r => r.OwnerUserId == user.Id);
                if (restaurant != null) restaurantId = restaurant.Id;
            }

            // 3. Bilgileri paketleyip gönder
            return Ok(new
            {
                Id = user.Id,
                Name = user.FullName,
                Role = user.Role,
                RestaurantId = restaurantId
            });
        }
    }
}