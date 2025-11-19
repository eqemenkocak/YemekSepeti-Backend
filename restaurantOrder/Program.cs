using Microsoft.EntityFrameworkCore;
using restaurantOrder.Models; // <-- En önemli kısım: Senin proje ismin küçük r ile başlıyor

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. Veritabanı Bağlantısı Ayarı
// ---------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<RestaurantOrderDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------
// 2. CORS Ayarı (React, API'ye erişebilsin diye ŞART)
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// ---------------------------------------------------------
// 3. Standart Servisler
// ---------------------------------------------------------
builder.Services.AddControllers();
// Veritabanı Bağlantı Servisi (Bu eksik olduğu için hata veriyor)
builder.Services.AddDbContext<restaurantOrder.Models.RestaurantsOrderDbContext>(options =>
    options.UseSqlServer("Server=EGEMENK38\\SQLEXPRESS;Database=RestaurantsOrderDB;Trusted_Connection=True;TrustServerCertificate=True;"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------------------------------------------------------
// 4. Uygulama Hattı (Pipeline)
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS'u burada devreye alıyoruz (React için)
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();