using Microsoft.AspNetCore.Mvc;
using restaurantOrder.Models;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Email veya şifre hatalı!" });
            }

            // GİRİŞ BAŞARILI! Frontend'e gönderilecek paket 👇
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,

                // BURASI EKSİKTİ, ŞİMDİ EKLİYORUZ:
                fullName = user.FullName, // Frontend bunu okuyup ekrana basacak

                restaurantId = 1, // Şimdilik 1. restoranın sahibi gibi davranıyoruz
                token = "dummy-token-12345" // JWT olmadığı için sahte token
            });
        }

        // Yardımcı Model
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}