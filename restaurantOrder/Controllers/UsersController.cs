using Microsoft.AspNetCore.Mvc;
using restaurantOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RestaurantOrderDbContext _context;

        public UsersController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        // KULLANICI BİLGİLERİNİ GETİR (Profil Sayfası açılınca dolması için)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Şifreyi göndermiyoruz (Güvenlik)
            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Phone,
                user.Role
            });
        }

        // KULLANICI GÜNCELLEME (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateProfileDto request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Bilgileri Güncelle
            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Email = request.Email;

            // Eğer yeni şifre girildiyse onu da güncelle
            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = request.Password; // Gerçek hayatta burası Hashlenmeli!
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profil başarıyla güncellendi!", user });
        }
    }
}