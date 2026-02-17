using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aplikasi_Manajemen_Sampah.Models
{
    public class Lokasi
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string Nama { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Keterangan { get; set; } = "";
    }
}
