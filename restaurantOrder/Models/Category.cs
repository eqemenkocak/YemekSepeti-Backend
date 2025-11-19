using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Category
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
