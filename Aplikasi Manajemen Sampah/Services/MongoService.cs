using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;

// PENTING: Wajib pakai 'Services' (ada huruf S di belakang) untuk konsistensi namespace
namespace Aplikasi_Manajemen_Sampah.Services
{
    /// <summary>
    /// Wrapper class untuk mengelola koneksi ke database MongoDB Atlas.
    /// Menyediakan akses terpusat ke semua koleksi data.
    /// </summary>
    public class MongoService
    {
        private readonly IMongoDatabase _db;

        /// <summary>
        /// Inisialisasi koneksi MongoDB menggunakan Connection String yang telah dikonfigurasi.
        /// </summary>
        public MongoService()
        {
            // Connection String ke MongoDB Atlas Cluster
            string connectionString = "mongodb+srv://lumbantoruansamuel07_db_user:samuel16@aplikasimanajemensampah.uljmyee.mongodb.net/?retryWrites=true&w=majority&appName=aplikasimanajemensampahdb";

            var client = new MongoClient(connectionString);
            _db = client.GetDatabase("ManajemenSampahDB");
        }

        /// <summary>
        /// Membuka akses langsung ke objek database MongoDB (misal untuk PdfService).
        /// </summary>
        public IMongoDatabase Database => _db;

        // --- Shortcut Properties untuk Akses Koleksi ---

        /// <summary>
        /// Koleksi data Pengguna (Akun & Role).
        /// </summary>
        public IMongoCollection<User> Users => _db.GetCollection<User>("Users");

        /// <summary>
        /// Koleksi data Sampah yang diinputkan pengguna.
        /// </summary>
        public IMongoCollection<Sampah> Sampah => _db.GetCollection<Sampah>("Sampah");

        /// <summary>
        /// Koleksi data transaksi Penjemputan sampah.
        /// </summary>
        public IMongoCollection<Penjemputan> Penjemputan => _db.GetCollection<Penjemputan>("Penjemputan");

        /// <summary>
        /// Koleksi riwayat percakapan Chatbot.
        /// </summary>
        public IMongoCollection<ChatHistory> ChatHistory => _db.GetCollection<ChatHistory>("ChatHistory");

        /// <summary>
        /// Koleksi data statistik referensi (Data Open Jabar).
        /// </summary>
        public IMongoCollection<DataSampah> DataSampah => _db.GetCollection<DataSampah>("DataSampah");
    }
}