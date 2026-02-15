using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System;
using System.Windows.Forms;
using System.Linq;

// PENTING: Wajib pakai 'Services'
namespace Aplikasi_Manajemen_Sampah.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _db;

        // STATUS OFFLINE
        public bool IsOffline { get; private set; } = false;

        public MongoService()
        {
            try
            {
                string connectionString =
                    "mongodb+srv://lumbantoruansamuel07_db_user:samuel16@aplikasimanajemensampah.uljmyee.mongodb.net/?retryWrites=true&w=majority&appName=aplikasimanajemensampahdb";

                var client = new MongoClient(connectionString);

                // TEST KONEKSI (agar cepat ketahuan gagal)
                client.ListDatabaseNames().ToList();

                _db = client.GetDatabase("ManajemenSampahDB");

                IsOffline = false;
            }
            catch (Exception)
            {
                // MODE OFFLINE
                IsOffline = true;

                MessageBox.Show(
                    "⚠️ Tidak bisa terhubung ke MongoDB.\nAplikasi berjalan dalam OFFLINE MODE.",
                    "Offline Mode",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ===========================
        // AKSES DATABASE (ONLINE ONLY)
        // ===========================
        public IMongoDatabase Database => _db;

        public IMongoCollection<User> Users =>
            _db?.GetCollection<User>("Users");

        public IMongoCollection<Sampah> Sampah =>
            _db?.GetCollection<Sampah>("Sampah");

        public IMongoCollection<Penjemputan> Penjemputan =>
            _db?.GetCollection<Penjemputan>("Penjemputan");
    }
}