using System;
using System.Drawing;
using System.Windows.Forms;
using Aplikasi_Manajemen_Sampah.Models;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Services;
using System.Collections.Generic; // Tambahkan ini untuk List

namespace Aplikasi_Manajemen_Sampah.Forms
{
    public partial class FormSampah : Form
    {
        private User currentUser;
        private MongoService mongo;
        private string selectedId = "";

        public FormSampah(User user)
        {
            this.currentUser = user;
            InitializeComponent();
            mongo = new MongoService();

            if (dgvSampah != null) UIHelper.SetGridStyle(dgvSampah);

            // 1. Panggil fungsi untuk mengisi data dropdown Jawa Barat
            IsiDataLokasiJawaBarat();

            SetupEvents();
            LoadData();
        }

        // --- FUNGSI BARU: Mengisi Dropdown Wilayah Jabar ---
        private void IsiDataLokasiJawaBarat()
        {
            // Pastikan cboLokasi sudah dibuat di Designer
            cboLokasi.Items.Clear();

            string[] wilayahJabar = {
                "Kota Bandung", "Kab. Bandung", "Kab. Bandung Barat",
                "Kota Bogor", "Kab. Bogor",
                "Kota Bekasi", "Kab. Bekasi",
                "Kota Depok",
                "Kota Cimahi",
                "Kota Tasikmalaya", "Kab. Tasikmalaya",
                "Kota Sukabumi", "Kab. Sukabumi",
                "Kota Cirebon", "Kab. Cirebon",
                "Kota Banjar",
                "Kab. Cianjur",
                "Kab. Garut",
                "Kab. Indramayu",
                "Kab. Karawang",
                "Kab. Kuningan",
                "Kab. Majalengka",
                "Kab. Pangandaran",
                "Kab. Purwakarta",
                "Kab. Subang",
                "Kab. Sumedang",
                "Kab. Ciamis"
            };

            cboLokasi.Items.AddRange(wilayahJabar);
            cboLokasi.SelectedIndex = 0; // Pilih default yang pertama
        }

        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnHapus.Click += BtnHapus_Click;
            btnClear.Click += (s, e) => ClearInputs();
            dgvSampah.CellClick += DgvSampah_CellClick;

            if (cboJenis.Items.Count > 0) cboJenis.SelectedIndex = 0;
        }

        private async void LoadData()
        {
            if (dgvSampah == null) return;

            try
            {
                var listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                dgvSampah.DataSource = listSampah;

                if (dgvSampah.Columns["Id"] != null) dgvSampah.Columns["Id"].Visible = false;
                if (dgvSampah.Columns["TanggalMasuk"] != null)
                    dgvSampah.Columns["TanggalMasuk"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
            catch (Exception ex) { MessageBox.Show($"Error loading data: {ex.Message}"); }
        }

        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            // Validasi Input (txtLokasi diganti cboLokasi)
            if (string.IsNullOrWhiteSpace(txtNama.Text) ||
                string.IsNullOrWhiteSpace(txtBerat.Text) ||
                cboLokasi.SelectedIndex == -1) // Cek apakah lokasi dipilih
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

            // Ambil text dari dropdown lokasi
            string lokasiTerpilih = cboLokasi.SelectedItem.ToString();
            string catatanOtomatis = "";

            // --- Logic ---

            // 1. Peringatan Limbah B3
            if (jenis == "B3")
            {
                var confirm = MessageBox.Show("⚠️ PERINGATAN LIMBAH B3!\nPastikan penanganan sesuai prosedur K3.\nLanjutkan?",
                    "Safety Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.No) return;
            }

            // 2. Auto-Note Daur Ulang
            if (jenis == "DaurUlang")
            {
                catatanOtomatis = " Perlu Dipisahkan (Daur Ulang)";
            }

            // 3. Peringatan Kapasitas
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
                    Lokasi = lokasiTerpilih, // Simpan dari dropdown
                    TanggalMasuk = DateTime.Now,
                    InputBy = currentUser.Username,
                    Catatan = catatanOtomatis
                };

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

            // Verifikasi Hapus B3
            if (cboJenis.SelectedItem?.ToString() == "B3")
            {
                if (MessageBox.Show("Hapus data B3 butuh verifikasi. Lanjutkan?", "Hapus B3", MessageBoxButtons.YesNo) == DialogResult.No) return;
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

            // --- Logic baru untuk Lokasi (Dropdown) ---
            string lokasiDb = row.Cells["Lokasi"].Value?.ToString();
            if (lokasiDb != null && cboLokasi.Items.Contains(lokasiDb))
            {
                cboLokasi.SelectedItem = lokasiDb;
            }
            else
            {
                // Jika lokasi di database tidak ada di list (misal data lama), tambahkan sementara atau pilih index 0
                // Opsi aman: Pilih index 0 atau biarkan kosong
                cboLokasi.Text = lokasiDb;
            }
            // ------------------------------------------

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

            // Reset Dropdown Lokasi
            if (cboLokasi.Items.Count > 0) cboLokasi.SelectedIndex = 0;

            cboJenis.SelectedIndex = 0;
            btnSimpan.Text = "Simpan";
            btnSimpan.BackColor = Color.FromArgb(46, 204, 113);
            txtNama.Focus();
        }
    }
}