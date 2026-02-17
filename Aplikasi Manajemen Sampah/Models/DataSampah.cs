using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aplikasi_Manajemen_Sampah.Models
{
    public class DataSampah
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        public string NamaProvinsi { get; set; } = "";
        public string NamaKabupatenKota { get; set; } = "";
        public double JumlahSampah { get; set; }
        public string Satuan { get; set; } = "";
        public int Tahun { get; set; }
    }
}
