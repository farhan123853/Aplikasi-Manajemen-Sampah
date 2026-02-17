using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using Aplikasi_Manajemen_Sampah.Models;
using Aplikasi_Manajemen_Sampah.Services;
using System.IO;
using System.Runtime.InteropServices; // Diperlukan untuk komunikasi COM interop
using System.Drawing;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form manajemen lokasi tambahan (Lokasi Custom).
    /// Mengintegrasikan Peta Leaflet.js melalui WebBrowser control untuk input koordinat interaktif.
    /// Kelas ini diberi atribut [ComVisible(true)] agar dapat dipanggil oleh JavaScript di dalam WebBrowser.
    /// </summary>
    [ComVisible(true)] 
    public partial class FormKelolaLokasi : Form
    {
        private MongoService mongo;
        private string selectedId = "";

        public FormKelolaLokasi()
        {
            InitializeComponent();
            mongo = new MongoService();
            
            // Konfigurasi WebBrowser agar bisa memanggil method C# (ObjectForScripting)
            webBrowser.ObjectForScripting = this; 
            
            LoadMap();
            LoadData();
            SetupEvents();
        }

        private void SetupEvents()
        {
            btnSimpan.Click += BtnSimpan_Click;
            btnReset.Click += (s, e) => ClearInputs();
            btnHapus.Click += BtnHapus_Click;
            dgvLokasi.CellClick += DgvLokasi_CellClick;
        }

        /// <summary>
        /// Memuat peta Leaflet.js dengan meng-generate file HTML sementara.
        /// Hal ini diperlukan karena WebBrowser di WinForms tidak support direct HTML string dengan external script modern dengan mudah.
        /// </summary>
        private void LoadMap()
        {
            try
            {
                // HTML Template Peta Leaflet
                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
    <style>
        body, html {{ height: 100%; margin: 0; }}
        #map {{ height: 100%; width: 100%; }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        // Inisialisasi Peta (Default View: Bandung)
        var map = L.map('map').setView([-6.9175, 107.6191], 10); 
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; OpenStreetMap contributors'
        }}).addTo(map);

        var currentMarker = null;

        // Fungsi JS untuk menaruh marker
        function setMarker(lat, lng) {{
            if (currentMarker) {{
                map.removeLayer(currentMarker);
            }}
            currentMarker = L.marker([lat, lng]).addTo(map);
            map.setView([lat, lng], 13);
        }}

        // Event Listener Klik Peta
        map.on('click', function(e) {{
            var lat = e.latlng.lat;
            var lng = e.latlng.lng;
            
            setMarker(lat, lng);

            // JEMBATAN KE C#: Memanggil method SetCoordinates di kode C#
            if (window.external) {{
                window.external.SetCoordinates(lat, lng);
            }}
        }});
    </script>
</body>
</html>";
                // Simpan ke file temp dan navigasi
                string tempPath = Path.Combine(Path.GetTempPath(), "map_input.html");
                File.WriteAllText(tempPath, htmlContent);
                webBrowser.Navigate(tempPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load map: " + ex.Message);
            }
        }

        /// <summary>
        /// Method callback yang dipanggil oleh JavaScript saat user mengklik peta.
        /// </summary>
        /// <param name="lat">Latitude dari peta</param>
        /// <param name="lng">Longitude dari peta</param>
        public void SetCoordinates(double lat, double lng)
        {
            // Update UI TextBox dengan format angka desimal yang konsisten (titik)
            txtLat.Text = lat.ToString().Replace(',', '.'); 
            txtLng.Text = lng.ToString().Replace(',', '.');
        }

        private async void LoadData()
        {
            try
            {
                var list = await mongo.Database.GetCollection<Lokasi>("Lokasi").Find(_ => true).ToListAsync();
                dgvLokasi.DataSource = list;

                if (dgvLokasi.Columns["Id"] != null) dgvLokasi.Columns["Id"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Gagal load data: " + ex.Message); }
        }

        private async void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNama.Text) || string.IsNullOrWhiteSpace(txtLat.Text) || string.IsNullOrWhiteSpace(txtLng.Text))
            {
                MessageBox.Show("Nama, Latitude, dan Longitude wajib diisi!");
                return;
            }

            // Parse Koordinat (Handling culture diff . vs ,)
            if (!double.TryParse(txtLat.Text.Replace('.', ','), out double lat) || 
                !double.TryParse(txtLng.Text.Replace('.', ','), out double lng))
            {
                MessageBox.Show("Format Latitude/Longitude salah!");
                return;
            }

            var lokasi = new Lokasi
            {
                Id = string.IsNullOrEmpty(selectedId) ? ObjectId.GenerateNewId().ToString() : selectedId,
                Nama = txtNama.Text,
                Latitude = lat,
                Longitude = lng,
                Keterangan = txtKeterangan.Text
            };

            var collection = mongo.Database.GetCollection<Lokasi>("Lokasi");

            if (string.IsNullOrEmpty(selectedId))
            {
                await collection.InsertOneAsync(lokasi);
            }
            else
            {
                await collection.ReplaceOneAsync(x => x.Id == selectedId, lokasi);
            }

            MessageBox.Show("Data berhasil disimpan!");
            ClearInputs();
            LoadData();
        }

        private async void BtnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedId)) return;
            
            if (MessageBox.Show("Hapus lokasi ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var collection = mongo.Database.GetCollection<Lokasi>("Lokasi");
                await collection.DeleteOneAsync(x => x.Id == selectedId);
                ClearInputs();
                LoadData();
            }
        }

        private void DgvLokasi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvLokasi.Rows[e.RowIndex];
            
            selectedId = row.Cells["Id"].Value.ToString();
            txtNama.Text = row.Cells["Nama"].Value.ToString();
            txtLat.Text = row.Cells["Latitude"].Value.ToString();
            txtLng.Text = row.Cells["Longitude"].Value.ToString();
            txtKeterangan.Text = row.Cells["Keterangan"].Value?.ToString();

            // Panggil Script JS untuk update posisi marker di peta saat edit
            if (double.TryParse(txtLat.Text.Replace('.', ','), out double lat) &&
                double.TryParse(txtLng.Text.Replace('.', ','), out double lng))
            {
                webBrowser.Document.InvokeScript("setMarker", new object[] { lat, lng });
            }

            btnSimpan.Text = "UPDATE";
            btnSimpan.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void ClearInputs()
        {
            selectedId = "";
            txtNama.Clear();
            txtLat.Clear();
            txtLng.Clear();
            txtKeterangan.Clear();
            btnSimpan.Text = "SIMPAN";
            btnSimpan.BackColor = Color.FromArgb(46, 204, 113);
        }
    }
}
