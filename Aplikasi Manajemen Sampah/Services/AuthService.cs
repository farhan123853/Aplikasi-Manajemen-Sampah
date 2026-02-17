using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System.Diagnostics;

namespace Aplikasi_Manajemen_Sampah.Services
{
    /// <summary>
    /// Layanan untuk menangani autentikasi dan manajemen akun pengguna.
    /// Mencakup registrasi user baru dan verifikasi login menggunakan hashing password.
    /// </summary>
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;

        /// <summary>
        /// Inisialisasi AuthService dan koneksi ke koleksi Users.
        /// </summary>
        public AuthService()
        {
            var mongo = new MongoService();
            _users = mongo.Users;
        }

        /// <summary>
        /// Mendaftarkan pengguna baru ke dalam sistem.
        /// Password akan di-hash menggunakan BCrypt sebelum disimpan.
        /// </summary>
        /// <param name="username">Nama pengguna yang diinginkan (harus unik).</param>
        /// <param name="plainPassword">Password asli (sebelum di-hash).</param>
        /// <param name="role">Peran pengguna (Default: "User"). Opsi lain: "Admin", "Petugas".</param>
        /// <returns>True jika berhasil dibuat.</returns>
        /// <exception cref="ArgumentException">Jika username/password kosong.</exception>
        /// <exception cref="InvalidOperationException">Jika username sudah terdaftar.</exception>
        public async Task<bool> CreateUserAsync(string username, string plainPassword, string role = "User")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(plainPassword))
                    throw new ArgumentException("Username dan Password wajib diisi.");

                // Cek duplikasi username
                var exists = await _users.Find(u => u.Username == username).AnyAsync();
                if (exists) throw new InvalidOperationException($"Username '{username}' sudah terdaftar.");

                var user = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword), // Security: Hashing password
                    Role = role
                };

                await _users.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error CreateUser: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Memverifikasi kredensial pengguna untuk login.
        /// </summary>
        /// <param name="username">Username yang diinput.</param>
        /// <param name="plainPassword">Password yang diinput.</param>
        /// <returns>Objek <see cref="User"/> jika valid, null jika gagal.</returns>
        public async Task<User?> LoginAsync(string username, string plainPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(plainPassword)) return null;

                var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
                if (user == null) return null; // User tidak ditemukan

                // Verifikasi hash password
                bool verified = BCrypt.Net.BCrypt.Verify(plainPassword, user.PasswordHash);
                return verified ? user : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Login: {ex.Message}");
                return null;
            }
        }
    }
}