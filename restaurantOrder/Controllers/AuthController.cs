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

        // --- DİKKAT: BURADA SADECE BİR TANE LOGIN METODU OLMALI ---

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Kullanıcıyı bul
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Email veya şifre hatalı!" });
            }

            // 2. BU KİŞİNİN SAHİP OLDUĞU RESTORANI BUL 🔍
            var myRestaurant = await _context.Restaurants
                                             .FirstOrDefaultAsync(r => r.OwnerUserId == user.Id);

            int myRestaurantId = myRestaurant != null ? myRestaurant.Id : 0;

            // 3. Frontend'e Doğru ID'yi Gönder
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                role = user.Role,
                fullName = user.FullName,
                restaurantId = myRestaurantId,
                token = "dummy-token-12345"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Bu e-posta adresi zaten kullanılıyor!" });
            }

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password,
                Phone = request.Phone,
                Role = "Customer"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Adres varsa ekle
            if (!string.IsNullOrEmpty(request.Address))
            {
                var newAddress = new Address
                {
                    UserId = newUser.Id,
                    Title = "Ev Adresi",
                    FullAddress = request.Address
                };
                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Kayıt başarılı! Lütfen giriş yapınız." });
        }

        // Yardımcı Model
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}