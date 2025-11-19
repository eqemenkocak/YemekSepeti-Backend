using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Address
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }

    public string FullAddress { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual User User { get; set; } = null!;
}
