using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;

// PENTING: Wajib pakai 'Services' (ada huruf S di belakang)
namespace Aplikasi_Manajemen_Sampah.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _db;

        public MongoService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _db = client.GetDatabase("ManajemenSampahDB");
        }

        // Membuka akses database untuk PdfService
        public IMongoDatabase Database => _db;

        // Shortcut Collection
        public IMongoCollection<User> Users => _db.GetCollection<User>("Users");
        public IMongoCollection<Sampah> Sampah => _db.GetCollection<Sampah>("Sampah");
        public IMongoCollection<Penjemputan> Penjemputan => _db.GetCollection<Penjemputan>("Penjemputan");
    }
}