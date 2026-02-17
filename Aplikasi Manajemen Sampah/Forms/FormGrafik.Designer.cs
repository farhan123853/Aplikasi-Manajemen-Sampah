namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class FormGrafik
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.btnFilter = new System.Windows.Forms.Button();
            this.cboJenis = new System.Windows.Forms.ComboBox();
            this.lblJenis = new System.Windows.Forms.Label();
            this.dtpSampai = new System.Windows.Forms.DateTimePicker();
            this.lblSampai = new System.Windows.Forms.Label();
            this.dtpDari = new System.Windows.Forms.DateTimePicker();
            this.lblDari = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.chartSampah = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblNoData = new System.Windows.Forms.Label();
            this.panelFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSampah)).BeginInit();
            this.SuspendLayout();
            //
            // panelFilter
            //
            this.panelFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(40)))));
            this.panelFilter.Controls.Add(this.btnFilter);
            this.panelFilter.Controls.Add(this.cboJenis);
            this.panelFilter.Controls.Add(this.lblJenis);
            this.panelFilter.Controls.Add(this.dtpSampai);
            this.panelFilter.Controls.Add(this.lblSampai);
            this.panelFilter.Controls.Add(this.dtpDari);
            this.panelFilter.Controls.Add(this.lblDari);
            this.panelFilter.Controls.Add(this.lblTitle);
            this.panelFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelFilter.Location = new System.Drawing.Point(0, 0);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(900, 100);
            this.panelFilter.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(220, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "📊 Statistik Limbah Harian";
            //
            // lblDari
            //
            this.lblDari.AutoSize = true;
            this.lblDari.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDari.ForeColor = System.Drawing.Color.White;
            this.lblDari.Location = new System.Drawing.Point(320, 18);
            this.lblDari.Name = "lblDari";
            this.lblDari.Size = new System.Drawing.Size(35, 19);
            this.lblDari.TabIndex = 1;
            this.lblDari.Text = "Dari:";
            //
            // dtpDari
            //
            this.dtpDari.CalendarFont = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpDari.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDari.Location = new System.Drawing.Point(360, 16);
            this.dtpDari.Name = "dtpDari";
            this.dtpDari.Size = new System.Drawing.Size(120, 23);
            this.dtpDari.TabIndex = 2;
            //
            // lblSampai
            //
            this.lblSampai.AutoSize = true;
            this.lblSampai.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSampai.ForeColor = System.Drawing.Color.White;
            this.lblSampai.Location = new System.Drawing.Point(490, 18);
            this.lblSampai.Name = "lblSampai";
            this.lblSampai.Size = new System.Drawing.Size(55, 19);
            this.lblSampai.TabIndex = 3;
            this.lblSampai.Text = "Sampai:";
            //
            // dtpSampai
            //
            this.dtpSampai.CalendarFont = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpSampai.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSampai.Location = new System.Drawing.Point(550, 16);
            this.dtpSampai.Name = "dtpSampai";
            this.dtpSampai.Size = new System.Drawing.Size(120, 23);
            this.dtpSampai.TabIndex = 4;
            //
            // btnFilter
            //
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnFilter.FlatAppearance.BorderSize = 0;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnFilter.ForeColor = System.Drawing.Color.White;
            this.btnFilter.Location = new System.Drawing.Point(360, 60);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 30);
            this.btnFilter.TabIndex = 5;
            this.btnFilter.Text = "🔍 Tampilkan";
            this.btnFilter.UseVisualStyleBackColor = false;
            //
            // lblJenis
            //
            this.lblJenis.AutoSize = true;
            this.lblJenis.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblJenis.ForeColor = System.Drawing.Color.White;
            this.lblJenis.Location = new System.Drawing.Point(15, 65);
            this.lblJenis.Name = "lblJenis";
            this.lblJenis.Size = new System.Drawing.Size(42, 19);
            this.lblJenis.TabIndex = 6;
            this.lblJenis.Text = "Jenis:";
            //
            // cboJenis
            //
            this.cboJenis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboJenis.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboJenis.FormattingEnabled = true;
            this.cboJenis.Location = new System.Drawing.Point(62, 63);
            this.cboJenis.Name = "cboJenis";
            this.cboJenis.Size = new System.Drawing.Size(280, 23);
            this.cboJenis.TabIndex = 7;
            //
            // chartSampah
            //
            chartArea1.Name = "ChartArea1";
            this.chartSampah.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartSampah.Legends.Add(legend1);
            this.chartSampah.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartSampah.Location = new System.Drawing.Point(0, 100);
            this.chartSampah.Name = "chartSampah";
            this.chartSampah.Size = new System.Drawing.Size(900, 350);
            this.chartSampah.TabIndex = 1;
            //
            // lblNoData
            //
            this.lblNoData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoData.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Italic);
            this.lblNoData.ForeColor = System.Drawing.Color.Gray;
            this.lblNoData.Location = new System.Drawing.Point(0, 100);
            this.lblNoData.Name = "lblNoData";
            this.lblNoData.Size = new System.Drawing.Size(900, 350);
            this.lblNoData.TabIndex = 2;
            this.lblNoData.Text = "Belum ada data sampah untuk rentang tanggal ini.";
            this.lblNoData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNoData.Visible = false;
            //
            // FormGrafik
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(900, 450);
            this.Controls.Add(this.chartSampah);
            this.Controls.Add(this.lblNoData);
            this.Controls.Add(this.panelFilter);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FormGrafik";
            this.Text = "Statistik Limbah";
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartSampah)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDari;
        private System.Windows.Forms.DateTimePicker dtpDari;
        private System.Windows.Forms.Label lblSampai;
        private System.Windows.Forms.DateTimePicker dtpSampai;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSampah;
        private System.Windows.Forms.Label lblNoData;
        private System.Windows.Forms.Label lblJenis;
        private System.Windows.Forms.ComboBox cboJenis;
    }
}