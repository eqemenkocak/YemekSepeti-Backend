using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models;
using YemekSepetim.Models;

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly RestaurantOrderDbContext _context;

        public RatingsController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] AddRatingDto ratingDto)
        {
            try
            {
                // 1. Validasyonlar (Aynen kalıyor)
                var order = await _context.Orders
                                          .Include(o => o.OrderItems)
                                          .FirstOrDefaultAsync(o => o.Id == ratingDto.OrderId);

                if (order == null) return NotFound(new { message = "Sipariş bulunamadı." });
                if (order.Status != "Teslim Edildi") return BadRequest(new { message = "Sadece teslim edilen siparişler puanlanabilir." });
                if (!order.OrderItems.Any(oi => oi.ProductId == ratingDto.ProductId)) return BadRequest(new { message = "Bu ürün bu siparişte yok." });

                var zatenPuanlamis = await _context.ProductRatings
                    .AnyAsync(r => r.OrderId == ratingDto.OrderId && r.ProductId == ratingDto.ProductId);
                if (zatenPuanlamis) return BadRequest(new { message = "Bu yemeği zaten puanladın." });

                // 2. Puanı Ekle (ProductRatings tablosunda trigger olmadığı için burası çalışır)
                var newRating = new ProductRating
                {
                    ProductId = ratingDto.ProductId,
                    UserId = order.CustomerId,
                    OrderId = ratingDto.OrderId,
                    Score = ratingDto.Score,
                    CreatedAt = DateTime.Now
                };

                _context.ProductRatings.Add(newRating);
                await _context.SaveChangesAsync();

                // 3. KRİTİK DÜZELTME BURADA 👇
                // Önce ortalamayı hesapla
                var yeniOrtalama = await _context.ProductRatings
                                                 .Where(r => r.ProductId == ratingDto.ProductId)
                                                 .AverageAsync(r => r.Score);

                // EF Core SaveChanges() YERİNE doğrudan SQL komutu atıyoruz.
                // Bu sayede 'Products' tablosundaki Fiyat Trigger'ı ile çakışmıyoruz.
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE Products SET AverageScore = {yeniOrtalama} WHERE Id = {ratingDto.ProductId}"
                );

                return Ok(new { message = "Puanın kaydedildi, teşekkürler!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Bir hata oluştu.",
                    errorDetail = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }
}