using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include için gerekli
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

        // POST: Sipariş Oluşturma
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            var newOrder = new Order
            {
                CustomerId = request.UserId,
                RestaurantId = request.RestaurantId,
                TotalAmount = request.TotalAmount,
                Status = "Bekleniyor...", // 👈 1. DEĞİŞİKLİK: İlk durum artık bu!
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

        // GET: Restorana Özel Siparişleri Getir (Detaylı)
        [HttpGet("ByRestaurant/{restaurantId}")]
        public IActionResult GetOrdersByRestaurant(int restaurantId)
        {
            var orders = _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.OrderItems)      // İlişkili tabloları dahil et
                .ThenInclude(oi => oi.Product)   // Ürün isimlerine ulaşmak için
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    // 👈 2. DEĞİŞİKLİK: Yemek isimlerini virgülle birleştirip gönderiyoruz
                    Items = o.OrderItems.Select(oi => oi.Product.Name).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        // PUT: Sipariş Durumunu Güncelle (YENİ ÖZELLİK 🚀)
        [HttpPut("UpdateStatus/{id}")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusRequest request)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = request.Status; // Yeni durumu yaz
            _context.SaveChanges(); // Kaydet

            return Ok(new { message = "Durum güncellendi" });
        }

        // Durum güncellemek için minik bir model
        public class StatusRequest
        {
            public string Status { get; set; }
        }
    }
}