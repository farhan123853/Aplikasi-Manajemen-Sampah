using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using Aplikasi_Manajemen_Sampah.Models;
using Aplikasi_Manajemen_Sampah.Services;
using System.IO;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form Visualisasi Peta Persebaran Sampah.
    /// Menggunakan WebBrowser control untuk me-render peta Leaflet.js.
    /// Menampilkan marker dinamis berdasarkan data sampah yang ada di database.
    /// </summary>
    public partial class FormMaps : Form
    {
        private MongoService mongo;
        private WebBrowser webBrowser;
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnRefresh;

        public FormMaps()
        {
            InitializeComponent();
            mongo = new MongoService();
            
            // Event Handlers untuk memastikan data selalu fresh saat form dibuka
            this.Load += (s, e) => LoadMapData();
            this.Activated += (s, e) => LoadMapData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = "Peta Wilayah Provinsi";
            this.BackColor = Color.White;

            // HEADER PANEL
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.White;
            this.Controls.Add(panelHeader);

            lblTitle = new Label();
            lblTitle.Text = "🗺️ Peta Wilayah Jawa Barat";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(46, 204, 113);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);
            panelHeader.Controls.Add(lblTitle);

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh Peta";
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(this.ClientSize.Width - 140, 12);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.BackColor = Color.FromArgb(52, 152, 219);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadMapData();
            panelHeader.Controls.Add(btnRefresh);

            // BUTTON DEBUG RESET SEED (Hanya untuk Development)
            var btnResetSeed = new Button();
            btnResetSeed.Text = "Reset Data"; // Tombol darurat untuk isi ulang data dummy
            btnResetSeed.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnResetSeed.Size = new Size(80, 35);
            btnResetSeed.Location = new Point(this.ClientSize.Width - 320, 12);
            btnResetSeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResetSeed.BackColor = Color.Orange;
            btnResetSeed.ForeColor = Color.White;
            btnResetSeed.FlatStyle = FlatStyle.Flat;
            btnResetSeed.Click += async (s, e) => 
            {
                if (MessageBox.Show("Reset database lokasi ke default? Data custom akan hilang.", "Debug Reset", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try {
                        var col = mongo.Database.GetCollection<Lokasi>("Lokasi");
                        await col.DeleteManyAsync(_ => true);
                        
                        var seedData = LocationData.BankSampahList.Select(x => new Lokasi {
                            Nama = x.Nama, Latitude = x.Lat, Longitude = x.Lng, Keterangan = "Default Reset"
                        }).ToList();
                        
                        await col.InsertManyAsync(seedData);
                        MessageBox.Show($"Reset Selesai! {seedData.Count} lokasi default dikembalikan.");
                        LoadMapData();
                    } catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                }
            };
            panelHeader.Controls.Add(btnResetSeed);

            // WEB BROWSER CONTROL (Wadah Peta)
            webBrowser = new WebBrowser();
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.ScriptErrorsSuppressed = true; // Mencegah popup error script bawaan IE
            this.Controls.Add(webBrowser);
        }

        /// <summary>
        /// Memuat data dari Database dan menghasilkan HTML Peta secara dinamis.
        /// Proses flow: Auto-seed -> Fetch Data -> Grouping -> Generate Markers -> Render HTML.
        /// </summary>
        private async void LoadMapData()
        {
            try
            {
                // 0. AUTO-SEEDING: Fitur Self-Healing
                // Jika koleksi Lokasi kosong, otomatis isi dengan data statis agar peta tidak blank.
                var collectionLokasi = mongo.Database.GetCollection<Lokasi>("Lokasi");
                long count = await collectionLokasi.CountDocumentsAsync(_ => true);
                
                if (count == 0)
                {
                    var seedData = new List<Lokasi>();
                    foreach(var item in LocationData.BankSampahList)
                    {
                        seedData.Add(new Lokasi 
                        { 
                            Nama = item.Nama, 
                            Latitude = item.Lat, 
                            Longitude = item.Lng, 
                            Keterangan = "Lokasi default sistem" 
                        });
                    }
                    if (seedData.Count > 0)
                        await collectionLokasi.InsertManyAsync(seedData);
                }

                // 1. Fetch Data
                var listSampah = await mongo.Sampah.Find(_ => true).ToListAsync();
                var listLokasi = await collectionLokasi.Find(_ => true).ToListAsync();

                // 2. Analisis Data Spasial
                // Mengelompokkan sampah berdasarkan Nama Lokasi untuk menghitung total beban per titik.
                var locationGroups = listSampah
                    .GroupBy(s => s.Lokasi)
                    .Select(g => new
                    {
                        Lokasi = g.Key,
                        TotalBerat = g.Sum(x => x.BeratKg),
                        JumlahPending = g.Count(x => x.Status == "Pending"),
                        JumlahDijemput = g.Count(x => x.Status == "Dijemput"),
                        ItemTerbanyak = g.OrderByDescending(x => x.BeratKg).FirstOrDefault()?.Nama ?? "-"
                    })
                    .ToList();

                // 3. Generate JavaScript Marker
                string markersJs = "";

                // --- Helper Internal ---
                
                // Helper SVG Generator: Membuat icon marker warna-warni secara on-the-fly tanpa file gambar eksternal.
                string GetPinSvg(string colorHex)
                {
                    try {
                        string svg = $@"<svg fill='{colorHex}' xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' width='36' height='36'><path d='M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z'/><path fill='none' d='M0 0h24v24H0z'/></svg>";
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(svg);
                        return "data:image/svg+xml;base64," + Convert.ToBase64String(bytes);
                    } catch { return ""; }
                }

                // Helper Koordinat Resolver: Mencocokkan nama lokasi string dengan koordinat Lat/Lng.
                (double Lat, double Lng)? GetCoords(string lokasiInput, List<Lokasi> dbLocations)
                {
                    // Prioritas 1: Cek database lokasi custom
                    var dbLoc = dbLocations.FirstOrDefault(l => string.Equals(l.Nama, lokasiInput, StringComparison.OrdinalIgnoreCase));
                    if (dbLoc != null) return (dbLoc.Latitude, dbLoc.Longitude);

                    // Prioritas 2: Cek data statis Kota/Kab
                    foreach (var kvp in LocationData.CityCoords)
                    {
                        if (string.Equals(kvp.Key, lokasiInput, StringComparison.OrdinalIgnoreCase) || 
                            lokasiInput.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            return kvp.Value;
                        }
                    }
                    
                    // Prioritas 3: Cek data statis Bank Sampah
                    var bank = LocationData.BankSampahList.FirstOrDefault(b => b.Nama == lokasiInput);
                    if (bank != default) return (bank.Lat, bank.Lng);

                    return null;
                }

                // LOOP A: Render Marker Transaksi Sampah
                foreach (var loc in locationGroups)
                {
                    var coords = GetCoords(loc.Lokasi, listLokasi);
                    if (coords.HasValue)
                    {
                        // Logika Warna Status: 
                        // Merah = Ada sampah numpuk (Pending)
                        // Hijau = Bersih (Dijemput semua)
                        // Biru = Netral / Tidak diketahui
                        string pinColor = "#3498db"; 
                        if (loc.JumlahPending > 0) pinColor = "#e74c3c"; 
                        else if (loc.JumlahDijemput > 0) pinColor = "#2ecc71"; 

                        string pinIconBase64 = GetPinSvg(pinColor);

                        string popupContent = $"<b>{loc.Lokasi}</b><br>" +
                                              $"Total Berat: {loc.TotalBerat} kg<br>" +
                                              $"<span style='color:red'>Pending: {loc.JumlahPending}</span><br>" +
                                              $"<span style='color:green'>Dijemput: {loc.JumlahDijemput}</span><br>" +
                                              $"Sampah Terberat: {loc.ItemTerbanyak}";

                        markersJs += $@"
                            var iconUser = L.icon({{
                                iconUrl: '{pinIconBase64}',
                                iconSize: [36, 36],
                                iconAnchor: [18, 36],
                                popupAnchor: [0, -34],
                                shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
                                shadowSize: [41, 41]
                            }});
                            L.marker([{coords.Value.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {coords.Value.Lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}], {{icon: iconUser}}).addTo(map).bindPopup(""{popupContent}"");
                        ";
                    }
                }

                // LOOP B: Render Marker Bank Sampah (Statis/Kantor) - Warna Ungu
                foreach(var bank in listLokasi)
                {
                    // Cek apakah lokasi ini sudah dirender di Loop A (overlap)
                    bool hasActivity = locationGroups.Any(g => string.Equals(g.Lokasi, bank.Nama, StringComparison.OrdinalIgnoreCase));

                    if (!hasActivity)
                    {
                        string popupContent = $"<b>{bank.Nama}</b><br>Kantor Bank Sampah<br><i>{bank.Keterangan}</i>";
                        string bankPinBase64 = GetPinSvg("#9b59b6"); // Ungu Khas

                        markersJs += $@"
                            var iconBank = L.icon({{
                                iconUrl: '{bankPinBase64}',
                                iconSize: [36, 36],
                                iconAnchor: [18, 36],
                                popupAnchor: [0, -34],
                                shadowUrl: 'https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png',
                                shadowSize: [41, 41]
                            }});
                            L.marker([{bank.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {bank.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}], {{icon: iconBank}}).addTo(map).bindPopup(""{popupContent}"");
                        ";
                    }
                }

                // 4. Konstruksi HTML Halaman Peta
                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
    <style>
        body, html {{ height: 100%; margin: 0; font-family: 'Segoe UI', sans-serif; }}
        #map {{ height: 100%; width: 100%; }}
        
        .legend {{
            padding: 8px 10px;
            font: 12px Arial, Helvetica, sans-serif;
            background: rgba(255,255,255,0.95);
            box-shadow: 0 0 15px rgba(0,0,0,0.2);
            border-radius: 5px;
            line-height: 24px;
            color: #333;
        }}
        .legend-icon {{
            width: 18px; height: 18px; float: left; margin-right: 8px; margin-top: 3px;
        }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script>
        var southWest = L.latLng(-8.20, 106.20);
        var northEast = L.latLng(-5.90, 109.00);
        var bounds = L.latLngBounds(southWest, northEast);

        var map = L.map('map', {{
            maxBounds: bounds,
            maxBoundsViscosity: 1.0,
            minZoom: 9, 
            maxZoom: 15
        }}).setView([-6.9175, 107.6191], 9);

        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '&copy; OpenStreetMap contributors',
            bounds: bounds
        }}).addTo(map);

        {markersJs}

        // GENERATE LEGENDA
        var legend = L.control({{position: 'bottomright'}});
        legend.onAdd = function (map) {{
            // Icon SVG Reuse
            var iconRed = '{GetPinSvg("#e74c3c")}';
            var iconGreen = '{GetPinSvg("#2ecc71")}';
            var iconViolet = '{GetPinSvg("#9b59b6")}';
            var iconBlue = '{GetPinSvg("#3498db")}';

            var div = L.DomUtil.create('div', 'legend');
            div.innerHTML += '<h4>Keterangan</h4>';
            div.innerHTML += '<div><img src=""' + iconRed + '"" class=""legend-icon""> Perlu Dijemput (Pending)</div>';
            div.innerHTML += '<div><img src=""' + iconGreen + '"" class=""legend-icon""> Sudah Dijemput (Selesai)</div>';
            div.innerHTML += '<div><img src=""' + iconBlue + '"" class=""legend-icon""> Lokasi Lain</div>';
            div.innerHTML += '<div><img src=""' + iconViolet + '"" class=""legend-icon""> <b>Kantor Bank Sampah</b></div>';
            return div;
        }};
        legend.addTo(map);

    </script>
</body>
</html>";

                // Tulis ke file temp dan load
                string tempPath = Path.Combine(Path.GetTempPath(), "map_sampah.html");
                File.WriteAllText(tempPath, htmlContent);
                webBrowser.Navigate(tempPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}");
            }
        }
    }
}
