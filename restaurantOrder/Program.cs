using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization; // <--- 1. KRİTİK EKLEME: Bu kütüphane şart!
using restaurantOrder.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. Veritabanı Bağlantısı
// ---------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<RestaurantOrderDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------
// 2. CORS Ayarı (React İçin)
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
// 3. Standart Servisler ve JSON Döngü Ayarı
// ---------------------------------------------------------

// 2. KRİTİK DÜZELTME BURADA 👇
// "ReferenceHandler.IgnoreCycles" diyerek sonsuz döngü hatasını engelliyoruz.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ... diğer servisler ...

// 👇 KALİTE KONTROL JOB'INI EKLİYORUZ
builder.Services.AddHostedService<restaurantOrder.Services.QualityControlService>();

// ...

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

// CORS Middleware
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();