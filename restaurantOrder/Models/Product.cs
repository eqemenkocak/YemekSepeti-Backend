using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    // Soru işareti (?) ekleyerek "Bunlar gönderilmese de olur, ben ID'den bulurum" diyoruz.
    public virtual Category? Category { get; set; }
    public virtual Restaurant? Restaurant { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Veritabanında decimal(3,1) yaptığımız için burada decimal kullanıyoruz.
    // Varsayılan değeri 0 olsun.
    public decimal AverageScore { get; set; }


}
