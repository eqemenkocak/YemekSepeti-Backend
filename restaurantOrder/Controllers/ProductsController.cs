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
    }
}