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
    public partial class FormGrafik : Form
    {
        private MongoService mongo;

        // Warna per jenis sampah
        private readonly Dictionary<string, Color> warnaJenis = new Dictionary<string, Color>
        {
            { "Organik",    Color.FromArgb(46, 204, 113) },   // Hijau
            { "Anorganik",  Color.FromArgb(52, 152, 219) },   // Biru
            { "B3",         Color.FromArgb(231, 76, 60) },    // Merah
            { "DaurUlang",  Color.FromArgb(241, 196, 15) }    // Kuning
        };

        public FormGrafik()
        {
            InitializeComponent();
            mongo = new MongoService();

            // Default range: 30 hari terakhir
            dtpDari.Value = DateTime.Now.AddDays(-30);
            dtpSampai.Value = DateTime.Now;

            // Isi ComboBox Jenis
            cboJenis.Items.AddRange(new object[] { "Semua Jenis", "Organik", "Anorganik", "B3", "DaurUlang" });
            cboJenis.SelectedIndex = 0; // Default: Semua Jenis

            // Event
            btnFilter.Click += (s, e) => LoadChartData();

            // Styling chart
            SetupChartStyle();

            // Load data pertama kali
            LoadChartData();
        }

        private void SetupChartStyle()
        {
            var area = chartSampah.ChartAreas[0];

            // Background
            chartSampah.BackColor = Color.FromArgb(245, 247, 250);
            area.BackColor = Color.FromArgb(245, 247, 250);

            // Axis X — menggunakan DateTime, label nama bulan
            area.AxisX.Title = "Bulan";
            area.AxisX.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisX.LabelStyle.Format = "MMM yyyy";
            area.AxisX.IntervalType = DateTimeIntervalType.Months;
            area.AxisX.Interval = 1;
            area.AxisX.MajorGrid.Enabled = true;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            area.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            // Axis Y
            area.AxisY.Title = "Berat (kg)";
            area.AxisY.TitleFont = new Font("Segoe UI", 10F, FontStyle.Bold);
            area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            area.AxisY.Minimum = 0;

            // Legend
            var legend = chartSampah.Legends[0];
            legend.Font = new Font("Segoe UI", 9F);
            legend.Docking = Docking.Top;
            legend.Alignment = StringAlignment.Center;

            // Title
            chartSampah.Titles.Clear();
            var title = new Title("Grafik Sampah Harian per Jenis", Docking.Top,
                new Font("Segoe UI", 13F, FontStyle.Bold), Color.FromArgb(30, 50, 40));
            chartSampah.Titles.Add(title);
        }

        private async void LoadChartData()
        {
            try
            {
                DateTime dari = dtpDari.Value.Date;
                DateTime sampai = dtpSampai.Value.Date.AddDays(1); // Inklusif hari terakhir

                // Validasi range
                if (dari > sampai)
                {
                    MessageBox.Show("Tanggal 'Dari' tidak boleh lebih besar dari 'Sampai'!",
                        "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Ambil data dari MongoDB
                var filter = Builders<Models.Sampah>.Filter.And(
                    Builders<Models.Sampah>.Filter.Gte(s => s.TanggalMasuk, dari),
                    Builders<Models.Sampah>.Filter.Lt(s => s.TanggalMasuk, sampai)
                );

                var listSampah = await mongo.Sampah.Find(filter).ToListAsync();

                // Cek apakah ada data
                if (listSampah == null || listSampah.Count == 0)
                {
                    chartSampah.Visible = false;
                    lblNoData.Visible = true;
                    return;
                }

                chartSampah.Visible = true;
                lblNoData.Visible = false;

                // Clear series lama
                chartSampah.Series.Clear();

                // Ambil semua tanggal unik, diurutkan
                var tanggalList = listSampah
                    .Select(s => s.TanggalMasuk.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Jenis sampah yang akan ditampilkan (filter berdasarkan ComboBox)
                string[] semuaJenis = { "Organik", "Anorganik", "B3", "DaurUlang" };
                string selectedJenis = cboJenis.SelectedItem?.ToString();

                string[] jenisTypes;
                if (selectedJenis != null && selectedJenis != "Semua Jenis")
                {
                    jenisTypes = new string[] { selectedJenis };
                }
                else
                {
                    jenisTypes = semuaJenis;
                }

                // Group data: per tanggal per jenis → total berat
                var grouped = listSampah
                    .GroupBy(s => new { Tanggal = s.TanggalMasuk.Date, s.Jenis })
                    .ToDictionary(
                        g => (g.Key.Tanggal, g.Key.Jenis),
                        g => g.Sum(x => x.BeratKg)
                    );

                // Buat Series per jenis
                foreach (var jenis in jenisTypes)
                {
                    var series = new Series(jenis)
                    {
                        ChartType = SeriesChartType.Line,
                        Color = warnaJenis.ContainsKey(jenis)
                            ? warnaJenis[jenis]
                            : Color.Gray,
                        BorderWidth = 3,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 8,
                        MarkerColor = warnaJenis.ContainsKey(jenis)
                            ? warnaJenis[jenis]
                            : Color.Gray,
                        IsValueShownAsLabel = false,
                        XValueType = ChartValueType.DateTime
                    };

                    // Tooltip — saat hover muncul tanggal lengkap
                    series.ToolTip = "#SERIESNAME\nTanggal: #VALX{dd/MM/yyyy}\nBerat: #VALY kg";

                    // Isi data per tanggal
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

                // Refresh chart
                chartSampah.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
