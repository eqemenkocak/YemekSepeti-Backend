using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class RestaurantAddress
{
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    public string FullAddress { get; set; } = null!;

    public virtual Restaurant Restaurant { get; set; } = null!;
}
