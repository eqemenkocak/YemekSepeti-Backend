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

        // 1. SİPARİŞ OLUŞTURMA
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequest request)
        {
            // Kullanıcının adresini bul (İlk bulduğunu al)
            var userAddress = _context.Addresses.FirstOrDefault(a => a.UserId == request.UserId);
            int addressToUse = userAddress != null ? userAddress.Id : 1;

            var newOrder = new Order
            {
                CustomerId = request.UserId,
                RestaurantId = request.RestaurantId,
                TotalAmount = request.TotalAmount,
                Status = "Bekleniyor...",
                PaymentMethod = request.PaymentMethod, // Ödeme yöntemi buradan geliyor
                AddressId = addressToUse
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

        // 2. ADMIN PANELİ İÇİN SİPARİŞLER (DÜZELTİLDİ ✅)
        [HttpGet("ByRestaurant/{restaurantId}")]
        public IActionResult GetOrdersByRestaurant(int restaurantId)
        {
            var orders = _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    CustomerName = o.Customer.FullName,
                    AddressText = o.Address != null ? o.Address.FullAddress : "Adres Bilgisi Yok",

                    // 👇 EKSİK PARÇA BURASIYDI! ARTIK GÖNDERİYORUZ:
                    PaymentMethod = o.PaymentMethod,

                    Items = o.OrderItems.Select(oi => new
                    {
                        Name = oi.Product.Name,
                        Score = _context.ProductRatings
                                    .Where(r => r.OrderId == o.Id && r.ProductId == oi.ProductId)
                                    .Select(r => r.Score)
                                    .FirstOrDefault()
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        // 3. DURUM GÜNCELLEME
        [HttpPut("UpdateStatus/{id}")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusRequest request)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = request.Status;
            _context.SaveChanges();

            return Ok(new { message = "Durum güncellendi" });
        }

        // 4. MÜŞTERİ SİPARİŞ GEÇMİŞİ (DÜZELTİLDİ ✅)
        [HttpGet("ByCustomer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .OrderByDescending(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    CustomerName = o.Customer.FullName,
                    AddressTitle = o.Address.Title ?? "Adres",
                    AddressText = o.Address.FullAddress,
                    RestaurantName = o.Restaurant.Name,

                    // 👇 EKSİK PARÇA BURASIYDI! ARTIK GÖNDERİYORUZ:
                    PaymentMethod = o.PaymentMethod,

                    Items = o.OrderItems.Select(oi => new
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }
        
        public class StatusRequest
        {
            public string Status { get; set; }
        }
    }
}