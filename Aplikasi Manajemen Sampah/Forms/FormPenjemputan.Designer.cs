namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class FormPenjemputan
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            panelInput = new Panel();
            webViewMap = new Microsoft.Web.WebView2.WinForms.WebView2();
            label5 = new Label();
            txtCatatan = new TextBox();
            label4 = new Label();
            cboStatus = new ComboBox();
            label3 = new Label();
            dtpTanggalJadwal = new DateTimePicker();
            btnClear = new RoundedButton();
            btnHapus = new RoundedButton();
            btnSimpan = new RoundedButton();
            cboPetugas = new ComboBox();
            lblPetugas = new Label();
            cboSampah = new ComboBox();
            lblSampah = new Label();
            lblTitleInput = new Label();
            dgvPenjemputan = new DataGridView();
            panelInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webViewMap).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvPenjemputan).BeginInit();
            SuspendLayout();
            // 
            // panelInput
            // 
            panelInput.BackColor = Color.White;
            panelInput.Controls.Add(webViewMap);
            panelInput.Controls.Add(label5);
            panelInput.Controls.Add(txtCatatan);
            panelInput.Controls.Add(label4);
            panelInput.Controls.Add(cboStatus);
            panelInput.Controls.Add(label3);
            panelInput.Controls.Add(dtpTanggalJadwal);
            panelInput.Controls.Add(btnClear);
            panelInput.Controls.Add(btnHapus);
            panelInput.Controls.Add(btnSimpan);
            panelInput.Controls.Add(cboPetugas);
            panelInput.Controls.Add(lblPetugas);
            panelInput.Controls.Add(cboSampah);
            panelInput.Controls.Add(lblSampah);
            panelInput.Controls.Add(lblTitleInput);
            panelInput.Dock = DockStyle.Left;
            panelInput.Location = new Point(0, 0);
            panelInput.Name = "panelInput";
            panelInput.Size = new Size(922, 600);
            panelInput.TabIndex = 0;
            // 
            // webViewMap
            // 
            webViewMap.AllowExternalDrop = true;
            webViewMap.CreationProperties = null;
            webViewMap.DefaultBackgroundColor = Color.White;
            webViewMap.Location = new Point(430, 110);
            webViewMap.Name = "webViewMap";
            webViewMap.Size = new Size(466, 202);
            webViewMap.TabIndex = 14;
            webViewMap.ZoomFactor = 1D;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(20, 350);
            label5.Name = "label5";
            label5.Size = new Size(49, 15);
            label5.TabIndex = 13;
            label5.Text = "Catatan";
            // 
            // txtCatatan
            // 
            txtCatatan.Font = new Font("Segoe UI", 10F);
            txtCatatan.Location = new Point(20, 370);
            txtCatatan.Multiline = true;
            txtCatatan.Name = "txtCatatan";
            txtCatatan.Size = new Size(290, 60);
            txtCatatan.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(20, 285);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 11;
            label4.Text = "Status";
            // 
            // cboStatus
            // 
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.Font = new Font("Segoe UI", 10F);
            cboStatus.FormattingEnabled = true;
            cboStatus.Items.AddRange(new object[] { "Terjadwal", "Selesai", "Dibatalkan" });
            cboStatus.Location = new Point(20, 305);
            cboStatus.Name = "cboStatus";
            cboStatus.Size = new Size(290, 25);
            cboStatus.TabIndex = 10;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(20, 220);
            label3.Name = "label3";
            label3.Size = new Size(89, 15);
            label3.TabIndex = 9;
            label3.Text = "Tanggal Jadwal";
            // 
            // dtpTanggalJadwal
            // 
            dtpTanggalJadwal.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpTanggalJadwal.Font = new Font("Segoe UI", 10F);
            dtpTanggalJadwal.Format = DateTimePickerFormat.Custom;
            dtpTanggalJadwal.Location = new Point(20, 240);
            dtpTanggalJadwal.Name = "dtpTanggalJadwal";
            dtpTanggalJadwal.Size = new Size(290, 25);
            dtpTanggalJadwal.TabIndex = 8;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.Gray;
            btnClear.Cursor = Cursors.Hand;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(230, 450);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 40);
            btnClear.TabIndex = 7;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            // 
            // btnHapus
            // 
            btnHapus.BackColor = Color.FromArgb(231, 76, 60);
            btnHapus.Cursor = Cursors.Hand;
            btnHapus.FlatAppearance.BorderSize = 0;
            btnHapus.FlatStyle = FlatStyle.Flat;
            btnHapus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnHapus.ForeColor = Color.White;
            btnHapus.Location = new Point(125, 450);
            btnHapus.Name = "btnHapus";
            btnHapus.Size = new Size(90, 40);
            btnHapus.TabIndex = 6;
            btnHapus.Text = "Hapus";
            btnHapus.UseVisualStyleBackColor = false;
            // 
            // btnSimpan
            // 
            btnSimpan.BackColor = Color.FromArgb(46, 204, 113);
            btnSimpan.Cursor = Cursors.Hand;
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.FlatStyle = FlatStyle.Flat;
            btnSimpan.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSimpan.ForeColor = Color.White;
            btnSimpan.Location = new Point(20, 450);
            btnSimpan.Name = "btnSimpan";
            btnSimpan.Size = new Size(90, 40);
            btnSimpan.TabIndex = 5;
            btnSimpan.Text = "Simpan";
            btnSimpan.UseVisualStyleBackColor = false;
            // 
            // cboPetugas
            // 
            cboPetugas.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPetugas.Font = new Font("Segoe UI", 10F);
            cboPetugas.FormattingEnabled = true;
            cboPetugas.Location = new Point(20, 175);
            cboPetugas.Name = "cboPetugas";
            cboPetugas.Size = new Size(290, 25);
            cboPetugas.TabIndex = 4;
            // 
            // lblPetugas
            // 
            lblPetugas.AutoSize = true;
            lblPetugas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPetugas.Location = new Point(20, 155);
            lblPetugas.Name = "lblPetugas";
            lblPetugas.Size = new Size(51, 15);
            lblPetugas.TabIndex = 3;
            lblPetugas.Text = "Petugas";
            // 
            // cboSampah
            // 
            cboSampah.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSampah.Font = new Font("Segoe UI", 10F);
            cboSampah.FormattingEnabled = true;
            cboSampah.Location = new Point(20, 110);
            cboSampah.Name = "cboSampah";
            cboSampah.Size = new Size(290, 25);
            cboSampah.TabIndex = 2;
            // 
            // lblSampah
            // 
            lblSampah.AutoSize = true;
            lblSampah.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSampah.Location = new Point(20, 90);
            lblSampah.Name = "lblSampah";
            lblSampah.Size = new Size(77, 15);
            lblSampah.TabIndex = 1;
            lblSampah.Text = "Pilih Sampah";
            // 
            // lblTitleInput
            // 
            lblTitleInput.AutoSize = true;
            lblTitleInput.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitleInput.ForeColor = Color.FromArgb(46, 204, 113);
            lblTitleInput.Location = new Point(20, 30);
            lblTitleInput.Name = "lblTitleInput";
            lblTitleInput.Size = new Size(202, 25);
            lblTitleInput.TabIndex = 0;
            lblTitleInput.Text = "Atur Penjemputan 🚛";
            // 
            // dgvPenjemputan
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvPenjemputan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvPenjemputan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvPenjemputan.DefaultCellStyle = dataGridViewCellStyle2;
            dgvPenjemputan.Dock = DockStyle.Fill;
            dgvPenjemputan.Location = new Point(922, 0);
            dgvPenjemputan.Name = "dgvPenjemputan";
            dgvPenjemputan.Size = new Size(78, 600);
            dgvPenjemputan.TabIndex = 1;
            // 
            // FormPenjemputan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(dgvPenjemputan);
            Controls.Add(panelInput);
            Font = new Font("Segoe UI", 9F);
            Name = "FormPenjemputan";
            Text = "Jadwal Penjemputan";
            panelInput.ResumeLayout(false);
            panelInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webViewMap).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvPenjemputan).EndInit();
            ResumeLayout(false);
        }
        #endregion

        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Label lblTitleInput;
        private System.Windows.Forms.Label lblSampah;
        private System.Windows.Forms.ComboBox cboSampah;
        private System.Windows.Forms.Label lblPetugas;
        private System.Windows.Forms.ComboBox cboPetugas;
        private RoundedButton btnSimpan;
        private RoundedButton btnHapus;
        private RoundedButton btnClear;
        private System.Windows.Forms.DateTimePicker dtpTanggalJadwal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCatatan;
        private System.Windows.Forms.DataGridView dgvPenjemputan;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewMap;
    }
}