using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Restaurant
{
    public int Id { get; set; }

    public int OwnerUserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? MinOrderPrice { get; set; }

    public int? AveragePrepTime { get; set; }

    public bool IsOpen { get; set; }

    public decimal? Rating { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User OwnerUser { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<RestaurantAddress> RestaurantAddresses { get; set; } = new List<RestaurantAddress>();

    
}
