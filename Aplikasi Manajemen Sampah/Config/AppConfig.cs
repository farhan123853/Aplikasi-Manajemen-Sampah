using System;

namespace Aplikasi_Manajemen_Sampah.Config
{
    /// <summary>
    /// Kelas konfigurasi aplikasi terpusat.
    /// Mengelola pemuatan variabel lingkungan (.env) dan penyediaan konstanta global seperti API Keys.
    /// </summary>
    public static class AppConfig
    {
        /// <summary>
        /// Memuat variabel lingkungan dari file `.env` ke dalam proses aplikasi.
        /// Method ini mencari file .env di direktori eksekusi (bin) dan direktori root proyek.
        /// </summary>
        public static void LoadEnv()
        {
            // Mulai pencarian dari direktori tempat aplikasi berjalan (Current Working Directory)
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            
            // Logika pencarian fallback:
            if (!File.Exists(dotenv))
            {
                // Jika tidak ada di bin/Debug, coba cari mundur ke folder root project
                // Berguna saat debugging di Visual Studio
                 var projectRoot = Directory.GetParent(root)?.Parent?.Parent?.FullName;
                 if (projectRoot != null) dotenv = Path.Combine(projectRoot, ".env");
            }

            // Jika file ditemukan, parse user line-by-line
            if (File.Exists(dotenv))
            {
                foreach (var line in File.ReadAllLines(dotenv))
                {
                    // Abaikan baris komentar atau kosong (logic sederhana)
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length != 2) continue;
                    
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
            }
        }

        /// <summary>
        /// Mengambil Mistral API Key dari Environment Variable.
        /// Jika environment variable belum diset (misal saat development awal), akan menggunakan fallback hardcoded key.
        /// </summary>
        public static string MistralApiKey
        {
            get
            {
                string apiKey = Environment.GetEnvironmentVariable("MISTRAL_API_KEY");
      
                if (string.IsNullOrEmpty(apiKey))
                {
                    // FALLBACK: Kosongkan jika tidak ada di .env (User wajib set .env)
                    apiKey = ""; 
           
                    System.Diagnostics.Debug.WriteLine("?? WARNING: MISTRAL_API_KEY environment variable tidak di-set.");
                }
                
                return apiKey;
            }
        }

        /// <summary>
        /// Endpoint URL untuk API Mistral AI (Chat Completions).
        /// </summary>
        public static string MistralApiUrl => "https://api.mistral.ai/v1/chat/completions";
    }
}
