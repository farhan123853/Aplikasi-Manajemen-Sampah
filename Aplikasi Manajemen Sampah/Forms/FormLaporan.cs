using System;
using System.Drawing; // Wajib ada untuk mengatur lokasi (Point)
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Services;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    public partial class FormLaporan : Form
    {
        public FormLaporan()
        {
            InitializeComponent();

            // 1. Setting Default Tanggal
            dtpMulai.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // Tanggal 1 bulan ini
            dtpSelesai.Value = DateTime.Now;

            // 2. Hubungkan Event untuk Logika Tata Letak
            this.Load += FormLaporan_Load;     // Saat form pertama kali dibuka
            this.Resize += FormLaporan_Resize; // Saat layar dibesarkan/dikecilkan

            // 3. Hubungkan Tombol
            btnCetak.Click += BtnCetak_Click;
        }

        // Fungsi ini otomatis dipanggil saat form dimuat
        private void FormLaporan_Load(object sender, EventArgs e)
        {
            AturPosisiTengah();
        }

        // Fungsi ini otomatis dipanggil saat ukuran layar berubah
        private void FormLaporan_Resize(object sender, EventArgs e)
        {
            AturPosisiTengah();
        }

        // --- LOGIKA SUPAYA POSISI SELALU DI TENGAH ---
        private void AturPosisiTengah()
        {
            if (panelMain != null)
            {
                // Rumus: (Lebar Layar - Lebar Panel) / 2
                int x = (this.ClientSize.Width - panelMain.Width) / 2;
                int y = (this.ClientSize.Height - panelMain.Height) / 2;

                // Set posisi baru
                panelMain.Location = new Point(x, y);
            }
        }
        // ---------------------------------------------

        private async void BtnCetak_Click(object sender, EventArgs e)
        {
            // Validasi Tanggal
            if (dtpMulai.Value.Date > dtpSelesai.Value.Date)
            {
                MessageBox.Show("Tanggal Mulai tidak boleh lebih besar dari Tanggal Selesai!", "Peringatan");
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                // Panggil Service dengan Nama Lengkap (Biar aman)
                var service = new Aplikasi_Manajemen_Sampah.Services.PdfService();

                // Kirim tanggal yang dipilih user ke fungsi export
                await service.ExportLaporanAsync(dtpMulai.Value, dtpSelesai.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal: " + ex.Message);
            }

            this.Cursor = Cursors.Default;
        }
    }
}