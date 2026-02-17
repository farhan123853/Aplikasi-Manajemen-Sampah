using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Aplikasi_Manajemen_Sampah.Services;
using MongoDB.Driver;

namespace Aplikasi_Manajemen_Sampah.Forms
{
    /// <summary>
    /// Form visualisasi statistik data sampah dalam bentuk Grafik Garis (Line Chart).
    /// Mendukung filter berdasarkan rentang tanggal dan jenis sampah.
    /// </summary>
    public partial class FormGrafik : Form
    {
        private MongoService mongo;

        // Definisi Palet Warna Konsisten untuk Jenis Sampah
        private readonly Dictionary<string, Color> warnaJenis = new Dictionary<string, Color>
        {
            { "Organik",    Color.FromArgb(46, 204, 113) },   // Hijau Alam
            { "Anorganik",  Color.FromArgb(52, 152, 219) },   // Biru Plastik
            { "B3",         Color.FromArgb(231, 76, 60) },    // Merah Bahaya
            { "DaurUlang",  Color.FromArgb(241, 196, 15) }    // Kuning Emas
        };

        public FormGrafik()
        {
            InitializeComponent();
            mongo = new MongoService();

            // Setup Default Filter: 30 Hari Terakhir
            dtpDari.Value = DateTime.Now.AddDays(-30);
            dtpSampai.Value = DateTime.Now;

            // Isi Dropdown Filter Jenis
            cboJenis.Items.AddRange(new object[] { "Semua Jenis", "Organik", "Anorganik", "B3", "DaurUlang" });
            cboJenis.SelectedIndex = 0; 

            // Hubungkan Event Handler
            btnFilter.Click += (s, e) => LoadChartData();

            SetupChartStyle();
            LoadChartData(); // Auto-load saat pertama buka
        }

        /// <summary>
        /// Mengkonfigurasi tampilan Chart Control (Kosmetik).
        /// Mengatur warna background, grid lines, format label axis, dan legend.
        /// </summary>
        private void SetupChartStyle()
        {
            var area = chartSampah.ChartAreas[0];

            // UI Modern: Background cerah lembut
            chartSampah.BackColor = Color.FromArgb(245, 247, 250);
            area.BackColor = Color.FromArgb(245, 247, 250);

            // Konfigurasi Sumbu X (Waktu)
            area.AxisX.Title = "Bulan";
            area.AxisX.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisX.LabelStyle.Format = "MMM yyyy"; // Format tanggal pendek
            area.AxisX.IntervalType = DateTimeIntervalType.Months; // Interval per bulan
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            // Konfigurasi Sumbu Y (Berat)
            area.AxisY.Title = "Berat (kg)";
            area.AxisY.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.Minimum = 0; // Mulai dari 0 Kg

            // Konfigurasi Legend
            var legend = chartSampah.Legends[0];
            legend.Font = new Font("Segoe UI", 9F);
            legend.Docking = Docking.Top;
            legend.Alignment = StringAlignment.Center;

            // Judul Grafik
            chartSampah.Titles.Clear();
            var title = new Title("Statistik Harian Limbah per Jenis", Docking.Top,
                new Font("Segoe UI", 13F, FontStyle.Bold), Color.FromArgb(30, 50, 40));
            chartSampah.Titles.Add(title);
        }

        /// <summary>
        /// Mengambil data dari MongoDB, melakukan aggregasi (grouping), dan me-render grafik.
        /// </summary>
        private async void LoadChartData()
        {
            try
            {
                DateTime dari = dtpDari.Value.Date;
                DateTime sampai = dtpSampai.Value.Date.AddDays(1); // Tambah 1 hari agar tanggal 'sampai' terhitung penuh (inklusif)

                // Validasi Tanggal
                if (dari > sampai)
                {
                    MessageBox.Show("Tanggal 'Dari' tidak boleh lebih besar dari 'Sampai'!",
                        "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Query ke MongoDB dengan Filter Rentang Waktu
                var filter = Builders<Models.Sampah>.Filter.And(
                    Builders<Models.Sampah>.Filter.Gte(s => s.TanggalMasuk, dari),
                    Builders<Models.Sampah>.Filter.Lt(s => s.TanggalMasuk, sampai)
                );

                var listSampah = await mongo.Sampah.Find(filter).ToListAsync();

                // Handling Data Kosong
                if (listSampah == null || listSampah.Count == 0)
                {
                    chartSampah.Visible = false;
                    lblNoData.Visible = true; // Tampilkan label "Data Kosong"
                    return;
                }

                chartSampah.Visible = true;
                lblNoData.Visible = false;

                chartSampah.Series.Clear();

                // Siapkan timeline Axis-X (Daftar semua tanggal unik yang ada datanya)
                var tanggalList = listSampah
                    .Select(s => s.TanggalMasuk.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Tentukan Jenis Sampah yang akan ditampilkan (Single vs Multi Series)
                string[] semuaJenis = { "Organik", "Anorganik", "B3", "DaurUlang" };
                string selectedJenis = cboJenis.SelectedItem?.ToString();

                List<string> jenisTypes = new List<string>();
                if (selectedJenis != null && selectedJenis != "Semua Jenis")
                {
                    jenisTypes.Add(selectedJenis);
                }
                else
                {
                    jenisTypes.AddRange(semuaJenis);
                }

                // Core Logic Aggregasi: Group by [Tanggal, Jenis] -> Sum(Berat)
                var grouped = listSampah
                    .GroupBy(s => new { Tanggal = s.TanggalMasuk.Date, s.Jenis })
                    .ToDictionary(
                        g => (g.Key.Tanggal, g.Key.Jenis),
                        g => g.Sum(x => x.BeratKg)
                    );

                // Render setiap Series (Garis)
                foreach (var jenis in jenisTypes)
                {
                    var series = new Series(jenis)
                    {
                        ChartType = SeriesChartType.Line,
                        Color = warnaJenis.ContainsKey(jenis) ? warnaJenis[jenis] : Color.Gray,
                        BorderWidth = 3,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 8,
                        MarkerColor = warnaJenis.ContainsKey(jenis) ? warnaJenis[jenis] : Color.Gray,
                        IsValueShownAsLabel = false,
                        XValueType = ChartValueType.DateTime
                    };

                    series.ToolTip = "#SERIESNAME\nTanggal: #VALX{dd/MM/yyyy}\nBerat: #VALY kg";

                    // Plot titik data. Jika tanggal tertentu tidak ada data, isi dengan 0
                    foreach (var tanggal in tanggalList)
                    {
                        double berat = 0;
                        if (grouped.ContainsKey((tanggal, jenis)))
                        {
                            berat = grouped[(tanggal, jenis)];
                        }

                        var point = new DataPoint();
                        point.SetValueXY(tanggal, berat);
                        series.Points.Add(point);
                    }

                    chartSampah.Series.Add(series);
                }

                chartSampah.Invalidate(); // Redraw chart
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
