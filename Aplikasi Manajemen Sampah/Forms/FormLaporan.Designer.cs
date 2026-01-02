namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class FormLaporan
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
            lblJudul = new Label();
            panelMain = new Panel();
            btnCetak = new RoundedButton();
            dtpSelesai = new DateTimePicker();
            label2 = new Label();
            dtpMulai = new DateTimePicker();
            label1 = new Label();
            panelMain.SuspendLayout();
            SuspendLayout();
            // 
            // lblJudul
            // 
            lblJudul.AutoSize = true;
            lblJudul.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblJudul.ForeColor = Color.FromArgb(46, 204, 113);
            lblJudul.Location = new Point(23, 27);
            lblJudul.Name = "lblJudul";
            lblJudul.Size = new Size(292, 37);
            lblJudul.TabIndex = 0;
            lblJudul.Text = "Laporan Transaksi 📄";
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.White;
            panelMain.Controls.Add(btnCetak);
            panelMain.Controls.Add(dtpSelesai);
            panelMain.Controls.Add(label2);
            panelMain.Controls.Add(dtpMulai);
            panelMain.Controls.Add(label1);
            panelMain.Location = new Point(178, 136);
            panelMain.Margin = new Padding(3, 4, 3, 4);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(571, 400);
            panelMain.TabIndex = 1;
            // 
            // btnCetak
            // 
            btnCetak.BackColor = Color.FromArgb(52, 152, 219);
            btnCetak.Cursor = Cursors.Hand;
            btnCetak.FlatAppearance.BorderSize = 0;
            btnCetak.FlatStyle = FlatStyle.Flat;
            btnCetak.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCetak.ForeColor = Color.White;
            btnCetak.Location = new Point(34, 267);
            btnCetak.Margin = new Padding(3, 4, 3, 4);
            btnCetak.Name = "btnCetak";
            btnCetak.Size = new Size(503, 67);
            btnCetak.TabIndex = 4;
            btnCetak.Text = "Cetak Laporan PDF";
            btnCetak.UseVisualStyleBackColor = false;
            // 
            // dtpSelesai
            // 
            dtpSelesai.Font = new Font("Segoe UI", 11F);
            dtpSelesai.Location = new Point(34, 187);
            dtpSelesai.Margin = new Padding(3, 4, 3, 4);
            dtpSelesai.Name = "dtpSelesai";
            dtpSelesai.Size = new Size(502, 32);
            dtpSelesai.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.ForeColor = Color.Gray;
            label2.Location = new Point(34, 153);
            label2.Name = "label2";
            label2.Size = new Size(139, 23);
            label2.TabIndex = 2;
            label2.Text = "Sampai Tanggal";
            // 
            // dtpMulai
            // 
            dtpMulai.Font = new Font("Segoe UI", 11F);
            dtpMulai.Location = new Point(34, 80);
            dtpMulai.Margin = new Padding(3, 4, 3, 4);
            dtpMulai.Name = "dtpMulai";
            dtpMulai.Size = new Size(502, 32);
            dtpMulai.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(34, 47);
            label1.Name = "label1";
            label1.Size = new Size(113, 23);
            label1.TabIndex = 0;
            label1.Text = "Dari Tanggal";
            // 
            // FormLaporan
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            BackColor = Color.FromArgb(245, 247, 250);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(914, 785);
            Controls.Add(panelMain);
            Controls.Add(lblJudul);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9F);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormLaporan";
            Text = "FormLaporan";
            Load += FormLaporan_Load;
            panelMain.ResumeLayout(false);
            panelMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblJudul;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpMulai;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpSelesai;
        private RoundedButton btnCetak;
    }
}