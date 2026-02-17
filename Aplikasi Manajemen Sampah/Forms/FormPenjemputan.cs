using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System.Linq;
using System.Collections.Generic;
using Aplikasi_Manajemen_Sampah.Services;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form untuk penjadwalan penjemputan sampah.
    /// Menghubungkan data Sampah dengan Petugas penjemput.
    /// Mengelola sinkronisasi status antara Penjemputan dan Data Sampah.
    /// </summary>
    public partial class FormPenjemputan : Form
    {
        private User currentUser;
        private MongoService mongo;

        private List<Sampah> listSampah = new List<Sampah>();
        private List<User> listPetugas = new List<User>();
        private string selectedId = "";

        public FormPenjemputan(User user)
        {
            this.currentUser = user;

            // Security: Hanya Admin dan Petugas yang boleh akses
            if (currentUser.Role == "User")
            {
                MessageBox.Show("Anda tidak memiliki akses ke halaman ini!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            this.mongo = new MongoService();

            InitializeComponent();

            if (dgvPenjemputan != null) UIHelper.SetGridStyle(dgvPenjemputan);

            SetupEvents();
            LoadComboData();
            LoadData();
        }

        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnHapus.Click += BtnHapus_Click;
            btnClear.Click += (s, e) => ClearInputs();
            dgvPenjemputan.CellClick += DgvPenjemputan_CellClick;
            cboStatus.SelectedIndexChanged += CboStatus_SelectedIndexChanged;

            cboStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// Memuat data referensi untuk Dropdown (Sampah & Petugas).
        /// </summary>
        private async void LoadComboData()
        {
            try
            {
                // Load Data Sampah untuk dipilih
                listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                cboSampah.Items.Clear();
                foreach (var s in listSampah) cboSampah.Items.Add($"{s.Nama} ({s.BeratKg} kg)");

                // Load Data Petugas untuk ditugaskan
                listPetugas = await mongo.Users.Find(u => u.Role == "Petugas" || u.Role == "Admin").ToListAsync();
                cboPetugas.Items.Clear();
                foreach (var p in listPetugas) cboPetugas.Items.Add(p.Username);

                // UX: Jika user yang login adalah Petugas, otomatis pilih dirinya sendiri
                if (currentUser.Role == "Petugas")
                {
                    var idx = listPetugas.FindIndex(p => p.Id == currentUser.Id);
                    if (idx >= 0) cboPetugas.SelectedIndex = idx;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Memuat daftar penjemputan.
        /// Melakukan "Manual Join" untuk mengambil Nama Sampah dan Nama Petugas dari ID masing-masing.
        /// </summary>
        private async void LoadData()
        {
            try
            {
                var data = await mongo.Penjemputan.Find(_ => true).ToListAsync();

                // Logic Join Data (MongoDB NoSQL tidak support Join relasional langsung yang efisien di layer aplikasi ini)
                foreach (var item in data)
                {
                    var s = await mongo.Sampah.Find(x => x.Id == item.SampahID).FirstOrDefaultAsync();
                    var p = await mongo.Users.Find(x => x.Id == item.PetugasID).FirstOrDefaultAsync();

                    item.NamaSampah = s?.Nama ?? "-";
                    item.LokasiSampah = s?.Lokasi ?? "-";
                    item.NamaPetugas = p?.Username ?? "-";
                }

                // Filter Data: Petugas biasa hanya bisa melihat jadwal miliknya
                if (currentUser.Role == "Petugas")
                {
                    data = data.Where(p => p.PetugasID == currentUser.Id).ToList();
                }

                dgvPenjemputan.DataSource = data;

                // Sembunyikan kolom ID referensi (Internal use only)
                string[] hiddenCols = { "Id", "SampahID", "PetugasID" };
                foreach (var col in hiddenCols)
                {
                    if (dgvPenjemputan.Columns[col] != null) dgvPenjemputan.Columns[col].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Gagal load data: " + ex.Message); }
        }

        /// <summary>
        /// Menyimpan jadwal penjemputan.
        /// Validasi overlap jadwal dan update status otomatis.
        /// </summary>
        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (cboSampah.SelectedIndex < 0 || cboPetugas.SelectedIndex < 0)
            {
                MessageBox.Show("Lengkapi data!"); return;
            }

            var sId = listSampah[cboSampah.SelectedIndex].Id;
            var pId = listPetugas[cboPetugas.SelectedIndex].Id;

            // Fitur Cerdas: Cek Bentrok Jadwal Petugas (< 2 jam selisih)
            bool overlap = await CheckOverlap(pId, dtpTanggalJadwal.Value, selectedId);
            if (overlap)
            {
                MessageBox.Show("❌ GAGAL: Petugas ini sudah ada jadwal di jam tersebut (Bentrokan)!");
                return;
            }

            var item = new Penjemputan
            {
                Id = string.IsNullOrEmpty(selectedId) ? MongoDB.Bson.ObjectId.GenerateNewId().ToString() : selectedId,
                SampahID = sId,
                PetugasID = pId,
                TanggalJadwal = dtpTanggalJadwal.Value,
                Status = cboStatus.SelectedItem.ToString(),
                Catatan = txtCatatan.Text
            };

            if (string.IsNullOrEmpty(selectedId))
                await mongo.Penjemputan.InsertOneAsync(item);
            else
                await mongo.Penjemputan.ReplaceOneAsync(x => x.Id == selectedId, item);

            // --- SINKRONISASI STATUS SAMPAH ---
            // Jika Penjemputan "Selesai", set status Sampah jadi "Dijemput" (Coret di tabel sampah)
            if (item.Status == "Selesai")
            {
                var updateSampahDef = Builders<Sampah>.Update.Set(s => s.Status, "Dijemput");
                await mongo.Sampah.UpdateOneAsync(s => s.Id == sId, updateSampahDef);
            }
            else
            {
                // Jika status revert dari Selesai ke pending, kembalikan status sampah juga
                var updateSampahDef = Builders<Sampah>.Update.Set(s => s.Status, "Pending");
                await mongo.Sampah.UpdateOneAsync(s => s.Id == sId, updateSampahDef);
            }
            // ----------------------------------

            MessageBox.Show("Berhasil disimpan!");
            ClearInputs();
            LoadData();
        }

        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId)) return;
            if (MessageBox.Show("Hapus?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await mongo.Penjemputan.DeleteOneAsync(x => x.Id == selectedId);
                ClearInputs();
                LoadData();
            }
        }

        private void DgvPenjemputan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvPenjemputan.Rows[e.RowIndex];
            selectedId = row.Cells["Id"].Value?.ToString();

            string sId = row.Cells["SampahID"].Value?.ToString();
            string pId = row.Cells["PetugasID"].Value?.ToString();

            cboSampah.SelectedIndex = listSampah.FindIndex(x => x.Id == sId);
            cboPetugas.SelectedIndex = listPetugas.FindIndex(x => x.Id == pId);

            dtpTanggalJadwal.Value = Convert.ToDateTime(row.Cells["TanggalJadwal"].Value);
            cboStatus.SelectedItem = row.Cells["Status"].Value?.ToString();
            txtCatatan.Text = row.Cells["Catatan"].Value?.ToString();

            btnSimpan.Text = "Update";
        }

        /// <summary>
        /// Mengecek apakah petugas memiliki jadwal lain dalam rentang waktu terdekat.
        /// Mencegah double-booking pada jam yang sama (toleransi 2 jam).
        /// </summary>
        private async Task<bool> CheckOverlap(string pId, DateTime date, string currentId)
        {
            try
            {
                var list = await mongo.Penjemputan.Find(x => x.PetugasID == pId).ToListAsync();

                // Exclude current record saat edit mode
                if (!string.IsNullOrEmpty(currentId))
                {
                    list = list.Where(x => x.Id != currentId).ToList();
                }

                // Overlap jika selisih waktu absolut < 2 jam
                return list.Any(x => Math.Abs((x.TanggalJadwal - date).TotalHours) < 2);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error CheckOverlap: " + ex.Message);
                return true; // Fail-safe: Anggap overlap jika error
            }
        }

        private void CboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Fitur User Experience: Auto-add note saat selesai
            if (cboStatus.SelectedItem?.ToString() == "Selesai")
                txtCatatan.Text += " [Tugas Selesai]";
        }

        private void ClearInputs()
        {
            selectedId = "";
            cboSampah.SelectedIndex = -1;
            if (currentUser.Role == "Admin") cboPetugas.SelectedIndex = -1;
            txtCatatan.Clear();
            btnSimpan.Text = "Simpan";
        }
    }
}