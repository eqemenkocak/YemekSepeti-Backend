using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models;

namespace restaurantOrder.Services
{
    public class QualityControlService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public QualityControlService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<RestaurantOrderDbContext>();

                    // 1. En düşük puanlı ürünleri bul (3.5 altı)
                    var riskyProducts = await context.Products
                        .Where(p => p.AverageScore > 0 && p.AverageScore < 3.5m)
                        .Include(p => p.Restaurant)
                        .ToListAsync();

                    Console.WriteLine($"\n[Kalite Kontrol {DateTime.Now:HH:mm:ss}]: Tarama Başladı...");

                    if (riskyProducts.Any())
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed; // Arka plan Kırmızı
                        Console.ForegroundColor = ConsoleColor.White;   // Yazı Beyaz
                        Console.WriteLine("!!! ALARM: DÜŞÜK KALİTE TESPİT EDİLDİ !!!");
                        Console.ResetColor(); // Renkleri sıfırla

                        foreach (var product in riskyProducts)
                        {
                            Console.WriteLine($"-> RESTORAN: {product.Restaurant.Name} | ÜRÜN: {product.Name} | PUAN: {product.AverageScore}");
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("-> Tüm yemeklerin durumu mükemmel. Sorun yok. ✅");
                        Console.ResetColor();
                    }
                }

                // 10 Saniye bekle (Hemen sonucu görmek için kısalttık)
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}