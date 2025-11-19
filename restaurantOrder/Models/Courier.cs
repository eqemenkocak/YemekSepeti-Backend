using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Courier
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? VehicleType { get; set; }

    public string? PlateNumber { get; set; }

    public virtual ICollection<DeliveryAssignment> DeliveryAssignments { get; set; } = new List<DeliveryAssignment>();

    public virtual User User { get; set; } = null!;
}
