namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class FormKelolaLokasi
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelInput = new System.Windows.Forms.Panel();
            this.lblTitleInput = new System.Windows.Forms.Label();
            this.txtNama = new System.Windows.Forms.TextBox();
            this.lblNama = new System.Windows.Forms.Label();
            this.txtLat = new System.Windows.Forms.TextBox();
            this.lblLat = new System.Windows.Forms.Label();
            this.txtLng = new System.Windows.Forms.TextBox();
            this.lblLng = new System.Windows.Forms.Label();
            this.txtKeterangan = new System.Windows.Forms.TextBox();
            this.lblKet = new System.Windows.Forms.Label();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            
            this.panelMap = new System.Windows.Forms.Panel();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.dgvLokasi = new System.Windows.Forms.DataGridView();

            this.panelInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLokasi)).BeginInit();
            this.SuspendLayout();

            // 
            // panelInput (Sidebar Kiri)
            // 
            this.panelInput.BackColor = System.Drawing.Color.White;
            this.panelInput.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelInput.Width = 300;
            this.panelInput.Padding = new System.Windows.Forms.Padding(20);
            this.panelInput.Controls.Add(this.lblTitleInput);
            this.panelInput.Controls.Add(this.lblNama);
            this.panelInput.Controls.Add(this.txtNama);
            this.panelInput.Controls.Add(this.lblLat);
            this.panelInput.Controls.Add(this.txtLat);
            this.panelInput.Controls.Add(this.lblLng);
            this.panelInput.Controls.Add(this.txtLng);
            this.panelInput.Controls.Add(this.lblKet);
            this.panelInput.Controls.Add(this.txtKeterangan);
            this.panelInput.Controls.Add(this.btnSimpan);
            this.panelInput.Controls.Add(this.btnReset);
            this.panelInput.Controls.Add(this.btnHapus);

            // Controls Setup
            this.lblTitleInput.Text = "KELOLA MASTER LOKASI";
            this.lblTitleInput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitleInput.ForeColor = System.Drawing.Color.Black;
            this.lblTitleInput.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitleInput.Height = 40;

            this.lblNama.Text = "Nama TPA atau TPS";
            this.lblNama.Location = new System.Drawing.Point(20, 60);
            this.lblNama.AutoSize = true;
            this.txtNama.Location = new System.Drawing.Point(20, 80);
            this.txtNama.Size = new System.Drawing.Size(260, 25);

            this.lblLat.Text = "Latitude";
            this.lblLat.Location = new System.Drawing.Point(20, 120);
            this.lblLat.AutoSize = true;
            this.txtLat.Location = new System.Drawing.Point(20, 140);
            this.txtLat.Size = new System.Drawing.Size(260, 25);

            this.lblLng.Text = "Longitude";
            this.lblLng.Location = new System.Drawing.Point(20, 180);
            this.lblLng.AutoSize = true;
            this.txtLng.Location = new System.Drawing.Point(20, 200);
            this.txtLng.Size = new System.Drawing.Size(260, 25);

            this.lblKet.Text = "Keterangan";
            this.lblKet.Location = new System.Drawing.Point(20, 240);
            this.lblKet.AutoSize = true;
            this.txtKeterangan.Location = new System.Drawing.Point(20, 260);
            this.txtKeterangan.Size = new System.Drawing.Size(260, 60);
            this.txtKeterangan.Multiline = true;

            this.btnSimpan.Text = "SIMPAN";
            this.btnSimpan.Location = new System.Drawing.Point(20, 340);
            this.btnSimpan.Size = new System.Drawing.Size(80, 40);
            this.btnSimpan.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnSimpan.ForeColor = System.Drawing.Color.White;
            this.btnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpan.FlatAppearance.BorderSize = 0;

            this.btnReset.Text = "RESET";
            this.btnReset.Location = new System.Drawing.Point(110, 340);
            this.btnReset.Size = new System.Drawing.Size(80, 40);
            this.btnReset.BackColor = System.Drawing.Color.Gray;
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.FlatAppearance.BorderSize = 0;

            this.btnHapus.Text = "HAPUS";
            this.btnHapus.Location = new System.Drawing.Point(200, 340);
            this.btnHapus.Size = new System.Drawing.Size(80, 40);
            this.btnHapus.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnHapus.ForeColor = System.Drawing.Color.White;
            this.btnHapus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHapus.FlatAppearance.BorderSize = 0;
            
            // 
            // panelMap (Kanan Atas)
            // 
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMap.Height = 400;
            this.panelMap.Controls.Add(this.webBrowser);

            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.ScriptErrorsSuppressed = true;

            // 
            // dgvLokasi (Kanan Bawah)
            // 
            this.dgvLokasi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLokasi.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dgvLokasi.AllowUserToAddRows = false;
            this.dgvLokasi.ReadOnly = true;
            this.dgvLokasi.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLokasi.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLokasi.ColumnHeadersHeight = 35;

            // 
            // FormKelolaLokasi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvLokasi);
            this.Controls.Add(this.panelMap);
            this.Controls.Add(this.panelInput);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Text = "Kelola Master Lokasi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLokasi)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Label lblTitleInput;
        private System.Windows.Forms.TextBox txtNama;
        private System.Windows.Forms.Label lblNama;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.Label lblLat;
        private System.Windows.Forms.TextBox txtLng;
        private System.Windows.Forms.Label lblLng;
        private System.Windows.Forms.TextBox txtKeterangan;
        private System.Windows.Forms.Label lblKet;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.DataGridView dgvLokasi;
    }
}
