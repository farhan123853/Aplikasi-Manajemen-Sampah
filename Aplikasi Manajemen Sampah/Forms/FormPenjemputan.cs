using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using System.Linq;
using System.Collections.Generic;
using Aplikasi_Manajemen_Sampah.Services;
using System.IO;
using Microsoft.Web.WebView2.Core;

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
            currentUser = user;
            mongo = new MongoService();

            InitializeComponent();

            InitMap();

            if (dgvPenjemputan != null)
                UIHelper.SetGridStyle(dgvPenjemputan);

            SetupEvents();
            LoadComboData();
            LoadData();
        }

        // ==========================
        // EVENT SETUP
        // ==========================
        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnHapus.Click += BtnHapus_Click;
            btnClear.Click += (s, e) => ClearInputs();

            dgvPenjemputan.CellClick += DgvPenjemputan_CellClick;
            cboStatus.SelectedIndexChanged += CboStatus_SelectedIndexChanged;
            cboSampah.SelectedIndexChanged += CboSampah_SelectedIndexChanged;

            cboStatus.SelectedIndex = 0;
        }

        // ==========================
        // INIT MAP
        // ==========================
        private async void InitMap()
        {
            try
            {
                await webViewMap.EnsureCoreWebView2Async(null);

                string path = Path.Combine(
                    Application.StartupPath,
                    "app",
                    "maps.html");

                if (File.Exists(path))
                    webViewMap.Source = new Uri(path);
                else
                    MessageBox.Show("maps.html tidak ditemukan!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load map: " + ex.Message);
            }
        }

        // ==========================
        // PINDAH MAP SAAT PILIH SAMPAH
        // ==========================
        private async void CboSampah_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSampah.SelectedIndex < 0) return;

            var sampah = listSampah[cboSampah.SelectedIndex];

            double lat = -6.9175;
            double lon = 107.6191;

            try
            {
                await webViewMap.ExecuteScriptAsync(
                    $"moveToLocation({lat},{lon},'{sampah.Nama}')");
            }
            catch { }
        }

        // ==========================
        // LOAD COMBO DATA
        // ==========================
        private async void LoadComboData()
        {
            try
            {
                if (mongo.IsOffline)
                {
                    listSampah = new List<Sampah>()
                    {
                        new Sampah{ Id="1", Nama="Sampah Rumah", BeratKg=5 },
                        new Sampah{ Id="2", Nama="Sampah Pasar", BeratKg=7 }
                    };
                }
                else
                {
                    listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                }

                cboSampah.Items.Clear();
                foreach (var s in listSampah)
                    cboSampah.Items.Add($"{s.Nama} ({s.BeratKg} kg)");

                if (!mongo.IsOffline)
                {
                    listPetugas = await mongo.Users
                        .Find(u => u.Role == "Petugas" || u.Role == "Admin")
                        .ToListAsync();

                    cboPetugas.Items.Clear();
                    foreach (var p in listPetugas)
                        cboPetugas.Items.Add(p.Username);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ==========================
        // LOAD DATA GRID
        // ==========================
        private async void LoadData()
        {
            try
            {
                if (mongo.IsOffline) return;

                var data = await mongo.Penjemputan.Find(_ => true).ToListAsync();

                foreach (var item in data)
                {
                    var s = await mongo.Sampah.Find(x => x.Id == item.SampahID).FirstOrDefaultAsync();
                    var p = await mongo.Users.Find(x => x.Id == item.PetugasID).FirstOrDefaultAsync();

                    item.NamaSampah = s?.Nama ?? "-";
                    item.LokasiSampah = s?.Lokasi ?? "-";
                    item.NamaPetugas = p?.Username ?? "-";
                }

                dgvPenjemputan.DataSource = data;

                string[] hiddenCols = { "Id", "SampahID", "PetugasID" };
                foreach (var col in hiddenCols)
                {
                    if (dgvPenjemputan.Columns[col] != null)
                        dgvPenjemputan.Columns[col].Visible = false;
                }
            }
            catch { }
        }

        // ==========================
        // SIMPAN DATA
        // ==========================
        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (mongo.IsOffline)
            {
                MessageBox.Show("OFFLINE MODE: data tidak disimpan ke database.");
                return;
            }

            if (cboSampah.SelectedIndex < 0 || cboPetugas.SelectedIndex < 0)
            {
                MessageBox.Show("Lengkapi data!");
                return;
            }

            var item = new Penjemputan
            {
                Id = string.IsNullOrEmpty(selectedId)
                    ? MongoDB.Bson.ObjectId.GenerateNewId().ToString()
                    : selectedId,
                SampahID = listSampah[cboSampah.SelectedIndex].Id,
                PetugasID = listPetugas[cboPetugas.SelectedIndex].Id,
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

        // ==========================
        // HAPUS DATA
        // ==========================
        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (mongo.IsOffline) return;
            if (string.IsNullOrEmpty(selectedId)) return;

            await mongo.Penjemputan.DeleteOneAsync(x => x.Id == selectedId);
            ClearInputs();
            LoadData();
        }

        private void DgvPenjemputan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvPenjemputan.Rows[e.RowIndex];
            selectedId = row.Cells["Id"].Value?.ToString();
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
            txtCatatan.Clear();
            btnSimpan.Text = "Simpan";
        }
    }
}