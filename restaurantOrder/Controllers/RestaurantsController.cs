using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models; // Model namespace'inin doğru olduğundan emin ol

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        // DÜZELTME: RestaurantsOrderDbContext -> RestaurantOrderDbContext (Tekil)
        private readonly RestaurantOrderDbContext _context;

        // Constructor'da da aynısını yapıyoruz:
        public RestaurantsController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        // GET: api/Restaurants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            // Restoranları getir
            return await _context.Restaurants.ToListAsync();
        }
        // ... Diğer metodların altına ...

        // ÖZEL RAPORLAMA METODU (Stored Procedure Kullanır)
        [HttpGet("Stats/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantStats(int restaurantId)
        {
            // SQL Prosedürünü çağırıyoruz
            var stats = await _context.Database
                .SqlQuery<StatsDto>($"EXEC sp_GetRestaurantStats @RestaurantId={restaurantId}")
                .ToListAsync();

            // Listeden ilk elemanı al (zaten tek satır dönüyor)
            var result = stats.FirstOrDefault();

            if (result == null)
            {
                return Ok(new StatsDto { Revenue = 0, OrderCount = 0, ProductCount = 0 });
            }

            return Ok(result);
        }
        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }
    }
}