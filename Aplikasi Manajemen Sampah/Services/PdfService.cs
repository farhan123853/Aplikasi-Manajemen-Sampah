using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Aplikasi_Manajemen_Sampah.Services
{
    /// <summary>
    /// Layanan untuk menghasilkan laporan dalam format PDF menggunakan library iText 7.
    /// Kelas ini menangani pengambilan data, pemformatan, dan penulisan dokumen laporan.
    /// </summary>
    public class PdfService
    {
        private readonly IMongoCollection<Sampah> _sampahCollection;
        private readonly IMongoCollection<User> _userCollection;

        /// <summary>
        /// Inisialisasi service dengan koneksi ke MongoDB collection yang relevan.
        /// </summary>
        public PdfService()
        {
            var mongo = new MongoService();
            _sampahCollection = mongo.Database.GetCollection<Sampah>("Sampah");
            _userCollection = mongo.Users;
        }

        /// <summary>
        /// Mengekspor laporan data sampah berdasarkan rentang tanggal dan peran pengguna.
        /// </summary>
        /// <param name="tglMulai">Tanggal awal periode laporan (inklusif).</param>
        /// <param name="tglAkhir">Tanggal akhir periode laporan (inklusif).</param>
        /// <param name="currentUser">Objek pengguna yang melakukan request. Digunakan untuk filter hak akses.</param>
        public async Task ExportLaporanAsync(DateTime tglMulai, DateTime tglAkhir, User currentUser = null)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "Laporan_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf";
            savefile.Filter = "PDF Files|*.pdf";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 1. FILTER BERDASARKAN TANGGAL DAN USER
                    var builder = Builders<Sampah>.Filter;

                    // Normalisasi range tanggal:
                    // Start = Jam 00:00:00 pada tanggal mulai
                    var start = tglMulai.Date;
                    // End = Jam 23:59:59 pada tanggal akhir (akhir hari)
                    // Menggunakan AddDays(1).AddTicks(-1) untuk memastikan seluruh data hari terakhir terambil.
                    var end = tglAkhir.Date.AddDays(1).AddTicks(-1);

                    var filter = builder.Gte(x => x.TanggalMasuk, start) &
                                 builder.Lte(x => x.TanggalMasuk, end);

                    // Penerapan Filter Keamanan Berdasarkan Role:
                    // Jika user adalah 'User' biasa (bukan Admin), filter data agar HANYA menampilkan data miliknya sendiri.
                    // Admin dapat melihat seluruh data.
                    if (currentUser != null && currentUser.Role != "Admin")
                    {
                        filter = filter & builder.Eq(x => x.InputBy, currentUser.Username);
                    }

                    var dataSampah = await _sampahCollection.Find(filter).ToListAsync();

                    if (dataSampah.Count == 0)
                    {
                        MessageBox.Show($"Tidak ada data sampah pada rentang tanggal:\n{start:dd/MM/yyyy} s/d {end:dd/MM/yyyy}", "Data Kosong");
                        return;
                    }

                    // Mengambil data seluruh pengguna untuk keperluan mapping Role.
                    // Dilakukan secara terpisah karena MongoDB tidak mendukung join relasional secara natif yang efisien di driver ini.
                    var users = await _userCollection.Find(_ => true).ToListAsync();
                    var userRoles = users.ToDictionary(u => u.Username, u => u.Role);

                    // 2. MULAI PROSES PEMBUATAN PDF
                    using (FileStream stream = new FileStream(savefile.FileName, FileMode.Create))
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdf = new PdfDocument(writer);
                        Document document = new Document(pdf);

                        // Header Laporan
                        document.Add(new Paragraph("Laporan Data Sampah")
                            .SetFontSize(20)
                            .SetTextAlignment(TextAlignment.CENTER));

                        // Informasi Periode Laporan
                        document.Add(new Paragraph($"Periode: {start:dd/MM/yyyy} - {end:dd/MM/yyyy}")
                            .SetFontSize(12)
                            .SetTextAlignment(TextAlignment.CENTER));

                        // Menampilkan metadata pembuat laporan jika bukan Admin (untuk transparansi)
                        if (currentUser != null && currentUser.Role != "Admin")
                        {
                            document.Add(new Paragraph($"User: {currentUser.Username} ({currentUser.Role})")
                                .SetFontSize(10)
                                .SetTextAlignment(TextAlignment.CENTER));
                        }

                        document.Add(new Paragraph("\n"));

                        // Konfigurasi Tabel Data (5 Kolom)
                        Table table = new Table(5);
                        table.SetWidth(UnitValue.CreatePercentValue(100));

                        // Header Tabel
                        table.AddHeaderCell("Tanggal");
                        table.AddHeaderCell("User (Role)");
                        table.AddHeaderCell("Item");
                        table.AddHeaderCell("Jenis");
                        table.AddHeaderCell("Berat (Kg)");

                        // Isi Tabel
                        foreach (var item in dataSampah)
                        {
                            table.AddCell(new Paragraph(item.TanggalMasuk.ToString("dd/MM/yyyy")));
                            
                            // Logika Format Tampilan User:
                            // Format target: "Nama (Role)" -> contoh "Budi (Petugas)"
                            string username = item.InputBy ?? "-";
                            string displayUser = username;
                            
                            // Kapitalisasi huruf pertama untuk estetika (misal: "admin" -> "Admin")
                            if (!string.IsNullOrEmpty(username) && username.Length > 1)
                                displayUser = char.ToUpper(username[0]) + username.Substring(1);

                            // Menambahkan info Role jika user ditemukan dalam dictionary roles
                            if (userRoles.ContainsKey(username))
                            {
                                displayUser += $" ({userRoles[username]})";
                            }
                            
                            table.AddCell(new Paragraph(displayUser));
                            table.AddCell(new Paragraph(item.Nama ?? "-"));
                            table.AddCell(new Paragraph(item.Jenis ?? "-"));
                            table.AddCell(new Paragraph(item.BeratKg.ToString()));
                        }

                        document.Add(table);
                        document.Close();
                    }

                    MessageBox.Show("Laporan PDF Berhasil Dibuat!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException)
                {
                    // Menangani kasus file sedang dibuka di aplikasi lain (misal Adobe Reader)
                    MessageBox.Show("Gagal menyimpan! File PDF sedang terbuka. Tutup dulu file tersebut.", "File Terkunci");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}