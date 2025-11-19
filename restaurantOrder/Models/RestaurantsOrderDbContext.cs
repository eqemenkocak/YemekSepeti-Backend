using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace restaurantOrder.Models;

public partial class RestaurantsOrderDbContext : DbContext
{
    public RestaurantsOrderDbContext()
    {
    }

    public RestaurantsOrderDbContext(DbContextOptions<RestaurantsOrderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Courier> Couriers { get; set; }

    public virtual DbSet<DeliveryAssignment> DeliveryAssignments { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<RestaurantAddress> RestaurantAddresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EGEMENK38\\SQLEXPRESS;Database=RestaurantsOrderDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addresse__3214EC07086CCCA9");

            entity.Property(e => e.FullAddress).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Addresses__UserI__5535A963");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07649292E0");

            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Categories)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Resta__5EBF139D");
        });

        modelBuilder.Entity<Courier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Couriers__3214EC07CE1E43E0");

            entity.Property(e => e.PlateNumber).HasMaxLength(20);
            entity.Property(e => e.VehicleType).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.Couriers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Couriers__UserId__6E01572D");
        });

        modelBuilder.Entity<DeliveryAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Delivery__3214EC07B6C877A0");

            entity.Property(e => e.IsCurrentAssignment).HasDefaultValue(true);

            entity.HasOne(d => d.Courier).WithMany(p => p.DeliveryAssignments)
                .HasForeignKey(d => d.CourierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeliveryA__Couri__72C60C4A");

            entity.HasOne(d => d.Order).WithMany(p => p.DeliveryAssignments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeliveryA__Order__71D1E811");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07C34E15EE");

            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__AddressI__6754599E");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Customer__656C112C");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Orders)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__Restaura__66603565");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC076803EFD1");

            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([Quantity]*[UnitPrice])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__6A30C649");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__6B24EA82");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC0742FA9AAE");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__619B8048");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.Products)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Restau__628FA481");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restaura__3214EC07B2E0C52E");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsOpen).HasDefaultValue(true);
            entity.Property(e => e.MinOrderPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Restaurants)
                .HasForeignKey(d => d.OwnerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Owner__59063A47");
        });

        modelBuilder.Entity<RestaurantAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restaura__3214EC07C1E789DA");

            entity.Property(e => e.FullAddress).HasMaxLength(500);

            entity.HasOne(d => d.Restaurant).WithMany(p => p.RestaurantAddresses)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Restauran__Resta__5BE2A6F2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC073EF53110");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105342CE5DC5E").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PasswordHash).HasMaxLength(512);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
