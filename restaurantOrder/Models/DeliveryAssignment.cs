using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class DeliveryAssignment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int CourierId { get; set; }

    public bool IsCurrentAssignment { get; set; }

    public virtual Courier Courier { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
