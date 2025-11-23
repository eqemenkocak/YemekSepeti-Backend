using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YemekSepetim.Models // Buradaki namespace senin projendekiyle aynı olsun
{
    public class ProductRating
    {
        [Key] // Bu tablonun birincil anahtarı (PK)
        public int Id { get; set; }

        public int ProductId { get; set; } // Hangi ürüne puan verildi?

        public int UserId { get; set; }    // Kim puan verdi?

        public int OrderId { get; set; }   // Hangi sipariş için?

        public int Score { get; set; }     // Kaç puan? (1-5)

        public string? Comment { get; set; } // Yorum (Boş olabilir o yüzden ?)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Ne zaman atıldı?
    }
}