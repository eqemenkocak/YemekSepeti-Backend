using System;
using System.Collections.Generic;

namespace restaurantOrder.Models;

public partial class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int RestaurantId { get; set; }

    public int AddressId { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<DeliveryAssignment> DeliveryAssignments { get; set; } = new List<DeliveryAssignment>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Restaurant Restaurant { get; set; } = null!;
}
