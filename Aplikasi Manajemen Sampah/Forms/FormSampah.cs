using System;
using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Services;
using System.Collections.Generic;
using System.Linq;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form untuk manajemen data sampah (CRUD).
    /// Mengelola input data sampah, lokasi pembuangan, dan jenis sampah.
    /// Dilengkapi fitur validasi input, peringatan limbah B3, dan pencatatan otomatis.
    /// </summary>
    public partial class FormSampah : Form
    {
        private User currentUser;
        private MongoService mongo;
        private string selectedId = "";

        public FormSampah(User user)
        {
            this.currentUser = user;

            // Security Check: Role 'User' biasa dibatasi aksesnya (Read-Only atau No Access)
            // Di sini implementasinya adalah blokir total.
            if (currentUser.Role == "User")
            {
                MessageBox.Show("Anda tidak memiliki akses ke halaman ini!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            InitializeComponent();
            mongo = new MongoService();

            if (dgvSampah != null) UIHelper.SetGridStyle(dgvSampah);

            // Inisialisasi Data Referensi
            IsiDataLokasiJawaBarat();

            SetupEvents();
            LoadData();
        }

        /// <summary>
        /// Mengisi dropdown lokasi dengan data wilayah administratif Jawa Barat.
        /// Sumber data prioritas: 
        /// 1. Data Statis (LocationData.CityCoords) - Cepat & Pasti.
        /// 2. Data Dinamis (Collection 'Lokasi') - Untuk lokasi custom tambahan.
        /// </summary>
        private async void IsiDataLokasiJawaBarat()
        {
            try
            {
                cboLokasi.Items.Clear();

                // 1. Load Wilayah Administratif (Kota/Kab)
                var daftarKotaJabar = LocationData.CityCoords.Keys.ToArray();
                cboLokasi.Items.AddRange(daftarKotaJabar);

                // 2. Load Lokasi Tambahan dari Database secara asinkron
                var listLokasiDb = await mongo.Database.GetCollection<Lokasi>("Lokasi").Find(_ => true).ToListAsync();
                
                foreach (var lokasi in listLokasiDb)
                {
                    // Hindari duplikasi jika nama kota sudah ada di list statis
                    if (!cboLokasi.Items.Contains(lokasi.Nama))
                    {
                        cboLokasi.Items.Add(lokasi.Nama);
                    }
                }

                if (cboLokasi.Items.Count > 0) cboLokasi.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data lokasi: " + ex.Message);
            }
        }

        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnHapus.Click += BtnHapus_Click;
            btnClear.Click += (s, e) => ClearInputs();
            dgvSampah.CellClick += DgvSampah_CellClick;
            dgvSampah.CellFormatting += DgvSampah_CellFormatting;

            if (cboJenis.Items.Count > 0) cboJenis.SelectedIndex = 0;
        }

        /// <summary>
        /// Memuat data sampah dari MongoDB ke DataGridView.
        /// </summary>
        private async void LoadData()
        {
            if (dgvSampah == null) return;

            try
            {
                var listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                dgvSampah.DataSource = listSampah;

                // Formatting Tampilan Grid
                if (dgvSampah.Columns["Id"] != null) dgvSampah.Columns["Id"].Visible = false;
                if (dgvSampah.Columns["TanggalMasuk"] != null)
                    dgvSampah.Columns["TanggalMasuk"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
            catch (Exception ex) { MessageBox.Show($"Error loading data: {ex.Message}"); }
        }

        /// <summary>
        /// Menangani logika penyimpanan data (Insert & Update).
        /// Mencakup validasi input, logika bisnis (B3 warning, Daur Ulang note), dan insert ke DB.
        /// </summary>
        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi Input Dasar
            if (string.IsNullOrWhiteSpace(txtNama.Text) ||
                string.IsNullOrWhiteSpace(txtBerat.Text) ||
                cboLokasi.SelectedIndex == -1)
            {
                MessageBox.Show("Nama, Berat, dan Lokasi wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtBerat.Text, out double beratKg))
            {
                MessageBox.Show("Berat harus berupa angka!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string jenis = cboJenis.SelectedItem?.ToString() ?? "Organik";
            string lokasiTerpilih = cboLokasi.SelectedItem.ToString();
            string catatanOtomatis = "";

            // --- LOGIKA BISNIS ---

            // 1. Safety Check: Limbah B3 (Berbahaya)
            if (jenis == "B3")
            {
                var confirm = MessageBox.Show("⚠️ PERINGATAN LIMBAH B3!\nPastikan penanganan sesuai prosedur K3.\nLanjutkan penyimpanan?",
                    "Safety Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }

            // 2. Auto-Note: Sampah Daur Ulang
            if (jenis == "DaurUlang")
            {
                catatanOtomatis = " Perlu Dipisahkan (Daur Ulang)";
            }

            // 3. Warning Kapasitas: Jika berat > 100kg
            if (beratKg >= 100)
            {
                MessageBox.Show("⚠️ KAPASITAS TINGGI DETEKSI!\nBerat > 100kg. Harap segera jadwalkan penjemputan.",
                    "Info Kapasitas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                var sampah = new Sampah
                {
                    Id = string.IsNullOrEmpty(selectedId) ? MongoDB.Bson.ObjectId.GenerateNewId().ToString() : selectedId,
                    Nama = txtNama.Text,
                    Jenis = jenis,
                    BeratKg = beratKg,
                    Lokasi = lokasiTerpilih,
                    TanggalMasuk = DateTime.Now,
                    InputBy = currentUser.Username,
                    Catatan = catatanOtomatis
                };

                // Insert or Update Logic
                if (string.IsNullOrEmpty(selectedId))
                {
                    await mongo.Sampah.InsertOneAsync(sampah);
                    MessageBox.Show("✓ Data berhasil disimpan!");
                }
                else
                {
                    await mongo.Sampah.ReplaceOneAsync(x => x.Id == selectedId, sampah);
                    MessageBox.Show("✓ Data berhasil diupdate!");
                }

                ClearInputs();
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Gagal menyimpan: " + ex.Message); }
        }

        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId))
            {
                MessageBox.Show("Pilih data dulu!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Verifikasi Ekstra untuk Hapus B3
            if (cboJenis.SelectedItem?.ToString() == "B3")
            {
                if (MessageBox.Show("Menghapus data B3 membutuhkan verifikasi ulang. Lanjutkan?", "Hapus B3", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            if (MessageBox.Show("Yakin hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await mongo.Sampah.DeleteOneAsync(x => x.Id == selectedId);
                ClearInputs();
                LoadData();
            }
        }

        private void DgvSampah_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvSampah.Rows[e.RowIndex];
            selectedId = row.Cells["Id"].Value?.ToString();
            txtNama.Text = row.Cells["Nama"].Value?.ToString();
            txtBerat.Text = row.Cells["BeratKg"].Value?.ToString();

            // Handling Lokasi di Dropdown saat Edit
            string lokasiDb = row.Cells["Lokasi"].Value?.ToString();
            if (lokasiDb != null && cboLokasi.Items.Contains(lokasiDb))
            {
                cboLokasi.SelectedItem = lokasiDb;
            }
            else
            {
                // Jika lokasi load dari DB tidak ada di list (misal data legacy), tampilkan saja sebagai text
                cboLokasi.Text = lokasiDb;
            }

            string jenis = row.Cells["Jenis"].Value?.ToString();
            if (cboJenis.Items.Contains(jenis)) cboJenis.SelectedItem = jenis;

            btnSimpan.Text = "Update";
            btnSimpan.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void ClearInputs()
        {
            selectedId = "";
            txtNama.Clear();
            txtBerat.Clear();

            // Reset Dropdown Lokasi ke default (Item pertama)
            if (cboLokasi.Items.Count > 0) cboLokasi.SelectedIndex = 0;

            cboJenis.SelectedIndex = 0;
            btnSimpan.Text = "Simpan";
            btnSimpan.BackColor = Color.FromArgb(46, 204, 113);
            txtNama.Focus();
        }

        /// <summary>
        /// Formatting visual pada baris tabel sesuai status data.
        /// Memberikan efek coret (strikeout) untuk sampah yang sudah dijemput.
        /// </summary>
        private void DgvSampah_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvSampah.Rows.Count)
            {
                var row = dgvSampah.Rows[e.RowIndex];
                var status = row.Cells["Status"].Value?.ToString();

                if (status == "Dijemput")
                {
                    // Efek Coret (Strikeout) untuk baris yang sudah dijemput
                    e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Strikeout);
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.SelectionForeColor = Color.LightGray;
                }
            }
        }
    }
}