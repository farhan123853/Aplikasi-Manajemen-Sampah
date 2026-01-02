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

        private async void LoadComboData()
        {
            try
            {
                // Load Data untuk Dropdown
                listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                cboSampah.Items.Clear();
                foreach (var s in listSampah) cboSampah.Items.Add($"{s.Nama} ({s.BeratKg} kg)");

                listPetugas = await mongo.Users.Find(u => u.Role == "Petugas" || u.Role == "Admin").ToListAsync();
                cboPetugas.Items.Clear();
                foreach (var p in listPetugas) cboPetugas.Items.Add(p.Username);

                // Auto-select jika user adalah Petugas
                if (currentUser.Role == "Petugas")
                {
                    var idx = listPetugas.FindIndex(p => p.Id == currentUser.Id);
                    if (idx >= 0) cboPetugas.SelectedIndex = idx;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private async void LoadData()
        {
            try
            {
                var data = await mongo.Penjemputan.Find(_ => true).ToListAsync();

                // Join Data Manual (mengambil nama sampah & petugas)
                foreach (var item in data)
                {
                    var s = await mongo.Sampah.Find(x => x.Id == item.SampahID).FirstOrDefaultAsync();
                    var p = await mongo.Users.Find(x => x.Id == item.PetugasID).FirstOrDefaultAsync();

                    item.NamaSampah = s?.Nama ?? "-";
                    item.LokasiSampah = s?.Lokasi ?? "-";
                    item.NamaPetugas = p?.Username ?? "-";
                }

                // Petugas hanya melihat datanya sendiri
                if (currentUser.Role == "Petugas")
                {
                    data = data.Where(p => p.PetugasID == currentUser.Id).ToList();
                }

                dgvPenjemputan.DataSource = data;

                string[] hiddenCols = { "Id", "SampahID", "PetugasID" };
                foreach (var col in hiddenCols)
                {
                    if (dgvPenjemputan.Columns[col] != null) dgvPenjemputan.Columns[col].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Gagal load data: " + ex.Message); }
        }

        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (cboSampah.SelectedIndex < 0 || cboPetugas.SelectedIndex < 0)
            {
                MessageBox.Show("Lengkapi data!"); return;
            }

            var sId = listSampah[cboSampah.SelectedIndex].Id;
            var pId = listPetugas[cboPetugas.SelectedIndex].Id;

            // Cek Tabrakan Jadwal
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

        private async Task<bool> CheckOverlap(string pId, DateTime date, string currentId)
        {
            try
            {
                // Ambil semua jadwal petugas tsb
                var list = await mongo.Penjemputan.Find(x => x.PetugasID == pId).ToListAsync();

                // Exclude ID yang sedang diedit (agar tidak bentrok dengan diri sendiri)
                if (!string.IsNullOrEmpty(currentId))
                {
                    list = list.Where(x => x.Id != currentId).ToList();
                }

                // Cek selisih waktu < 2 jam
                return list.Any(x => Math.Abs((x.TanggalJadwal - date).TotalHours) < 2);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error CheckOverlap: " + ex.Message);
                return true;
            }
        }

        private void CboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
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