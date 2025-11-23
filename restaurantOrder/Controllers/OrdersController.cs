using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models;

namespace restaurantOrder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RestaurantOrderDbContext _context;

        public OrdersController(RestaurantOrderDbContext context)
        {
            _context = context;
        }

        // 1. SİPARİŞ OLUŞTURMA (POST)
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            var newOrder = new Order
            {
                CustomerId = request.UserId,
                RestaurantId = request.RestaurantId,
                TotalAmount = request.TotalAmount,
                Status = "Bekleniyor...",
                PaymentMethod = "Kredi Kartı",
                AddressId = 1
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var productId in request.ProductIds)
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = newOrder.Id,
                        ProductId = productId,
                        Quantity = 1,
                        UnitPrice = product.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }
            _context.SaveChanges();

            return Ok(new { message = "Sipariş başarıyla alındı!", orderId = newOrder.Id });
        }

        // 2. RESTORANA ÖZEL SİPARİŞLER (GET)
        [HttpGet("ByRestaurant/{restaurantId}")]
        public IActionResult GetOrdersByRestaurant(int restaurantId)
        {
            var orders = _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    Items = o.OrderItems.Select(oi => oi.Product.Name).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        // 3. SİPARİŞ DURUMU GÜNCELLEME (PUT)
        [HttpPut("UpdateStatus/{id}")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusRequest request)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = request.Status;
            _context.SaveChanges();

            return Ok(new { message = "Durum güncellendi" });
        }

        // 4. MÜŞTERİYE ÖZEL SİPARİŞLER (BU METOD SINIFIN İÇİNDE OLMALIYDI) 👇
        // 4. MÜŞTERİYE ÖZEL SİPARİŞLER (DÜZELTİLMİŞ VERSİYON)
        [HttpGet("ByCustomer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    // DÜZELTME BURADA: Artık sadece isim değil, ID + İsim gönderiyoruz
                    Items = o.OrderItems.Select(oi => new
                    {
                        ProductId = oi.ProductId,      // <--- Puan vermek için bu LAZIM!
                        ProductName = oi.Product.Name
                    }).ToList(),
                    RestaurantName = o.Restaurant.Name
                })
                .ToList();

            return Ok(orders);
        }

        // Yardımcı Model (Class içinde kalabilir)
        public class StatusRequest
        {
            public string Status { get; set; }
        }

    } // Class Burada Bitiyor ✅
} // Namespace Burada Bitiyor ✅