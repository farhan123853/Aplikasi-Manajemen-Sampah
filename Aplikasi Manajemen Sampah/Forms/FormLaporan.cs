using System;
using System.Drawing; 
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Services;
using Aplikasi_Manajemen_Sampah.Models;
using MongoDB.Driver;
using System.Linq; // Penting untuk fungsi agregat (Max, Min)

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form antarmuka untuk pembuatan Laporan PDF.
    /// User memilih rentang tanggal, dan form akan memicu PdfService.
    /// Dilengkapi fitur auto-detect tanggal berdasarkan data yang ada di database.
    /// </summary>
    public partial class FormLaporan : Form
    {
        private User currentUser;
        private MongoService mongo;

        public FormLaporan(User user)
        {
            this.currentUser = user;
            mongo = new MongoService();
            InitializeComponent();

            // 1. Setting Default Tanggal: Otomatis mendeteksi rentang waktu data aktif
            SetDefaultDateRange();

            // 2. Hubungkan Event untuk Responsivitas Layout
            this.Load += FormLaporan_Load;     
            this.Resize += FormLaporan_Resize; 

            // 3. Setup Action Button
            btnCetak.Click += BtnCetak_Click;
        }

        /// <summary>
        /// Mengatur nilai default DatePicker berdasarkan tanggal data paling awal dan paling akhir di Database.
        /// Memudahkan user agar tidak perlu menebak periode laporan.
        /// </summary>
        private async void SetDefaultDateRange()
        {
            try
            {
                var allData = await mongo.Sampah.Find(_ => true).ToListAsync();

                if (allData.Count > 0)
                {
                    // Cari tanggal maksimum (terbaru) dan minimum (terlama)
                    var latestDate = allData.Max(x => x.TanggalMasuk);
                    var earliestDate = allData.Min(x => x.TanggalMasuk);

                    // Set DatePicker sesuai range data yang ditemukan
                    dtpMulai.Value = earliestDate.Date;
                    dtpSelesai.Value = latestDate.Date;
                }
                else
                {
                    // Fallback jika database kosong: Default ke bulan berjalan
                    dtpMulai.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    dtpSelesai.Value = DateTime.Now;
                }
            }
            catch
            {
                // Error Handler: Default aman ke bulan berjalan
                dtpMulai.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtpSelesai.Value = DateTime.Now;
            }
        }

        private void FormLaporan_Load(object sender, EventArgs e)
        {
            AturPosisiTengah();
        }

        private void FormLaporan_Resize(object sender, EventArgs e)
        {
            AturPosisiTengah();
        }

        /// <summary>
        /// Menjaga panel konten tetap berada di tengah form saat ukuran window diubah (Responsif).
        /// </summary>
        private void AturPosisiTengah()
        {
            if (panelMain != null)
            {
                // Rumus Centering: (LebarForm - LebarPanel) / 2
                int x = (this.ClientSize.Width - panelMain.Width) / 2;
                int y = (this.ClientSize.Height - panelMain.Height) / 2;

                panelMain.Location = new Point(x, y);
            }
        }

        /// <summary>
        /// Handler tombol Cetak. Memvalidasi input dan memanggil PdfService untuk generate file.
        /// </summary>
        private async void BtnCetak_Click(object sender, EventArgs e)
        {
            // Validasi Logika Tanggal
            if (dtpMulai.Value.Date > dtpSelesai.Value.Date)
            {
                MessageBox.Show("Tanggal Mulai tidak boleh lebih besar dari Tanggal Selesai!", "Peringatan");
                return;
            }

            this.Cursor = Cursors.WaitCursor; // Indikator loading

            try
            {
                var service = new PdfService();

                // Panggil service export dengan parameter tanggal dan user context (untuk header laporan)
                await service.ExportLaporanAsync(dtpMulai.Value, dtpSelesai.Value, currentUser);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal membuat laporan: " + ex.Message, "Error Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
        }
    }
}