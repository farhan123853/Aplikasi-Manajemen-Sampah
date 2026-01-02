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
            string connectionString = "mongodb+srv://lumbantoruansamuel07_db_user:samuel16@aplikasimanajemensampah.uljmyee.mongodb.net/?retryWrites=true&w=majority&appName=aplikasimanajemensampahdb";

            var client = new MongoClient(connectionString);
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