using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;

namespace Aplikasi_Manajemen_Sampah.Services
{
    /// <summary>
    /// Layanan untuk mengelola data referensi sampah (Statistik Jawa Barat).
    /// Menangani impor data dari CSV, seeding data awal, dan penyediaan knowledge base untuk chatbot.
    /// </summary>
    public class DataSampahService
    {
        private readonly MongoService _mongo;

        public DataSampahService()
        {
            _mongo = new MongoService();
        }

        /// <summary>
        /// Mengimpor data statistik sampah dari file CSV (Format Open Data Jabar).
        /// Format CSV yang didukung: 
        /// 1. Standar: nama_provinsi, nama_kabupaten_kota, jumlah_sampah, satuan, tahun
        /// 2. Lengkap: id, kode_provinsi, nama_provinsi, kode_kabupaten_kota, nama_kabupaten_kota, jumlah_sampah, satuan, tahun
        /// </summary>
        /// <param name="filePath">Lokasi absolut file CSV yang akan diimpor.</param>
        /// <returns>Jumlah baris data yang berhasil diimpor.</returns>
        /// <exception cref="FileNotFoundException">Jika file tidak ditemukan.</exception>
        public int ImportFromCsv(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File tidak ditemukan: {filePath}");

            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1)
                throw new Exception("File CSV kosong atau hanya berisi header.");

            var dataList = new List<DataSampah>();

            // Iterasi mulai dari index 1 untuk melewati header
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] cols = line.Split(',');
                
                try
                {
                    var data = new DataSampah();

                    // CASE A: Format Lengkap Open Data Jabar (8 kolom)
                    // id, kode_provinsi, nama_provinsi, kode_kabupaten_kota, nama_kabupaten_kota, jumlah_sampah, satuan, tahun
                    if (cols.Length >= 8)
                    {
                        data.NamaProvinsi = cols[2].Trim().Trim('"');
                        data.NamaKabupatenKota = cols[4].Trim().Trim('"');
                        data.JumlahSampah = double.Parse(cols[5].Trim().Trim('"'), CultureInfo.InvariantCulture);
                        data.Satuan = cols[6].Trim().Trim('"');
                        data.Tahun = int.Parse(cols[7].Trim().Trim('"'));
                    }
                    // CASE B: Format Sederhana (5 kolom)
                    // nama_provinsi, nama_kabupaten_kota, jumlah_sampah, satuan, tahun
                    else if (cols.Length >= 5)
                    {
                        data.NamaProvinsi = cols[0].Trim().Trim('"');
                        data.NamaKabupatenKota = cols[1].Trim().Trim('"');
                        
                        // Validasi kolom kedua bukan angka (untuk menghindari kesalahan urutan kolom)
                        if (double.TryParse(cols[1], out _)) throw new Exception("Format kolom salah");

                        data.JumlahSampah = double.Parse(cols[2].Trim().Trim('"'), CultureInfo.InvariantCulture);
                        data.Satuan = cols[3].Trim().Trim('"');
                        data.Tahun = int.Parse(cols[4].Trim().Trim('"'));
                    }
                    else
                    {
                        continue; // Lewati baris jika jumlah kolom tidak sesuai
                    }

                    dataList.Add(data);
                }
                catch
                {
                    // Fail-safe: Lewati baris yang gagal di-parse agar proses import tidak berhenti total
                    continue;
                }
            }

            if (dataList.Count > 0)
            {
                // Reset data lama sebelum memasukkan data baru (Full Refresh)
                _mongo.DataSampah.DeleteMany(FilterDefinition<DataSampah>.Empty);
                _mongo.DataSampah.InsertMany(dataList);
            }

            return dataList.Count;
        }

        /// <summary>
        /// Mengisi database dengan data awal (Seeding) jika kosong.
        /// Strategi prioritas:
        /// 1. Cek file CSV lokal ('Data/jabar_sampah.csv').
        /// 2. Jika tidak ada file, gunakan data hardcoded sebagai fallback.
        /// </summary>
        public void SeedDefaultData()
        {
            try
            {
                // Deteksi lokasi file CSV di berbagai kemungkinan path (bin/Debug vs Project Root)
                string permanentCsvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "jabar_sampah.csv");
                
                if (!File.Exists(permanentCsvPath))
                {
                    // Coba cari di folder project (naik 2-3 level dari bin)
                    var parentDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent;
                    if (parentDir != null)
                    {
                        string devPath = Path.Combine(parentDir.FullName, "Data", "jabar_sampah.csv");
                        if (File.Exists(devPath))
                        {
                            permanentCsvPath = devPath;
                        }
                    }
                }

                if (File.Exists(permanentCsvPath))
                {
                    long dbCount = _mongo.DataSampah.CountDocuments(FilterDefinition<DataSampah>.Empty);
                    
                    // Jika DB kosong, lakukan import otomatis dari CSV
                    if (dbCount == 0)
                    {
                        ImportFromCsv(permanentCsvPath);
                        return; // Selesai, menggunakan data dari CSV
                    }
                }
            }
            catch { /* Abaikan error pada pengecekan file, lanjutkan ke fallback */ }

            // Default Hardcoded Data (Fallback jika CSV tidak ditemukan)
            long count = _mongo.DataSampah.CountDocuments(FilterDefinition<DataSampah>.Empty);
            if (count > 0) return; // Database sudah terisi, tidak perlu seeding

            var defaultData = new List<DataSampah>
            {
                // Data sampel Hardcoded dari Open Data Jabar (Tahun 2015)
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN BOGOR", JumlahSampah = 861919.33, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN SUKABUMI", JumlahSampah = 205690.23, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN CIANJUR", JumlahSampah = 355554.29, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN BANDUNG", JumlahSampah = 1156523, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN GARUT", JumlahSampah = 237389.82, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN TASIKMALAYA", JumlahSampah = 293210.71, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN CIAMIS", JumlahSampah = 94302.11, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN KUNINGAN", JumlahSampah = 156700.50, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN CIREBON", JumlahSampah = 312450.80, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN MAJALENGKA", JumlahSampah = 178230.45, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN SUMEDANG", JumlahSampah = 145890.33, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN INDRAMAYU", JumlahSampah = 267500.90, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN SUBANG", JumlahSampah = 198340.22, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN PURWAKARTA", JumlahSampah = 134560.78, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN KARAWANG", JumlahSampah = 345670.55, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KABUPATEN BEKASI", JumlahSampah = 789230.40, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA BANDUNG", JumlahSampah = 1500340.50, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA BOGOR", JumlahSampah = 356780.12, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA SUKABUMI", JumlahSampah = 98450.33, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA CIREBON", JumlahSampah = 145230.67, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA BEKASI", JumlahSampah = 678900.45, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA DEPOK", JumlahSampah = 534210.88, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA CIMAHI", JumlahSampah = 189340.22, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA TASIKMALAYA", JumlahSampah = 123450.90, Satuan = "TON PER HARI", Tahun = 2015 },
                new DataSampah { NamaProvinsi = "JAWA BARAT", NamaKabupatenKota = "KOTA BANJAR", JumlahSampah = 45670.33, Satuan = "TON PER HARI", Tahun = 2015 },
            };

            _mongo.DataSampah.InsertMany(defaultData);
        }

        /// <summary>
        /// Mengambil seluruh knowledge base dari database dan memformatnya menjadi string teks.
        /// Digunakan sebagai konteks (RAG) untuk Chatbot AI agar dapat menjawab pertanyaan spesifik daerah.
        /// </summary>
        /// <returns>String terformat berisi ringkasan data sampah per tahun dan peringkat kota.</returns>
        public string GetKnowledgeBaseFromDb()
        {
            var allData = _mongo.DataSampah.Find(FilterDefinition<DataSampah>.Empty)
                .SortByDescending(d => d.Tahun)
                .ThenBy(d => d.NamaKabupatenKota)
                .ToList();

            if (allData.Count == 0)
                return "Belum ada data sampah di database.";

            var result = "=== DATA SAMPAH JAWA BARAT (dari Database) ===\n\n";

            // Mengelompokkan data berdasarkan Tahun untuk struktur laporan yang hierarkis
            var byYear = allData.GroupBy(d => d.Tahun).OrderByDescending(g => g.Key);

            foreach (var yearGroup in byYear)
            {
                result += $"📊 TAHUN {yearGroup.Key}:\n";

                double totalSampah = yearGroup.Sum(d => d.JumlahSampah);
                result += $"  Total sampah Jawa Barat: {totalSampah:N2} ton\n\n";

                // Menampilkan 5 Daerah Penghasil Sampah Terbanyak (Insight Penting)
                var top5 = yearGroup.OrderByDescending(d => d.JumlahSampah).Take(5);
                result += $"  🏆 Top 5 Penghasil Sampah Terbanyak:\n";
                int rank = 1;
                foreach (var d in top5)
                {
                    result += $"    {rank}. {d.NamaKabupatenKota}: {d.JumlahSampah:N2} {d.Satuan.ToLower()}\n";
                    rank++;
                }

                result += $"\n  📋 Data lengkap per Kabupaten/Kota:\n";
                foreach (var d in yearGroup.OrderBy(d => d.NamaKabupatenKota))
                {
                    result += $"    • {d.NamaKabupatenKota}: {d.JumlahSampah:N2} {d.Satuan.ToLower()}\n";
                }
                result += "\n";
            }

            return result;
        }

        /// <summary>
        /// Menghitung jumlah total dokumen data sampah di database.
        /// </summary>
        /// <returns>Jumlah dokumen.</returns>
        public long GetDataCount()
        {
            return _mongo.DataSampah.CountDocuments(FilterDefinition<DataSampah>.Empty);
        }
    }
}
