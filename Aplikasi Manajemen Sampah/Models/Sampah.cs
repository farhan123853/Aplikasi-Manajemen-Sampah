using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aplikasi_Manajemen_Sampah.Models
{
    public class Sampah
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string Nama { get; set; } = "";

        // Organik, Anorganik, B3, DaurUlang
        public string Jenis { get; set; } = "Organik";

        public double BeratKg { get; set; } = 0;
        public string Lokasi { get; set; } = "";
        public DateTime TanggalMasuk { get; set; } = DateTime.Now;
        public string InputBy { get; set; } = "";
        public string Catatan { get; set; } = "";
        
        // Status Penjemputan: "Pending", "Dijemput"
        public string Status { get; set; } = "Pending";
    }
}