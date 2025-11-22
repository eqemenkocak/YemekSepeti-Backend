using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models;

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Context ismin attığın koda göre 'RestaurantOrderDbContext'
        // Eğer hata verirse 'RestaurantsOrderDbContext' (s takısı ile) dene.
        private readonly RestaurantOrderDbContext _context;

        public ProductsController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ÜRÜNLERİ GETİREN METOD
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // 2. İŞTE EKSİK OLAN PARÇA BU! (Bunu ekliyoruz)
        // Sadece belirli bir restoranın ürünlerini getirir
        [HttpGet("ByRestaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByRestaurant(int restaurantId)
        {
            var products = await _context.Products
                                         .Where(p => p.RestaurantId == restaurantId)
                                         .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("Bu restorana ait ürün bulunamadı.");
            }

            return products;


        }

        // 3. YENİ ÜRÜN EKLEME (CREATE)
        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Veritabanına ekle
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Başarılı kodu (201 Created) döndür
            return CreatedAtAction("GetProducts", new { id = product.Id }, product);
        }

        // 4. ÜRÜN SİLME (DELETE)
        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Silinecek ürün bulunamadı.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); // Başarılı ama geriye veri dönmüyorum demek
        }
    }
}