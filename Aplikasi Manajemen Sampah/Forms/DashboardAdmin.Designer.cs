namespace Aplikasi_Manajemen_Sampah.Forms
{
    partial class DashboardAdmin
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            // 1. INI PENTING: Membuat Tombol Cetak
            this.btnCetak = new System.Windows.Forms.Button();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnPenjemputan = new System.Windows.Forms.Button();
            this.btnSampah = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.lblTitleSidebar = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelSidebar.SuspendLayout();
            this.panelLogo.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(40)))));
            // 2. INI PENTING: Menempelkan Tombol ke Sidebar
            // Urutan Add menentukan posisi tumpukan (Stack)
            this.panelSidebar.Controls.Add(this.btnCetak); // <-- Pastikan baris ini ada!
            this.panelSidebar.Controls.Add(this.btnUsers);
            this.panelSidebar.Controls.Add(this.btnPenjemputan);
            this.panelSidebar.Controls.Add(this.btnSampah);
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Controls.Add(this.panelLogo);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(240, 673);
            this.panelSidebar.TabIndex = 0;
            // 
            // btnCetak (Setting Tampilan Tombol)
            // 
            this.btnCetak.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCetak.FlatAppearance.BorderSize = 0;
            this.btnCetak.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCetak.ForeColor = System.Drawing.Color.White;
            this.btnCetak.Location = new System.Drawing.Point(0, 250);
            this.btnCetak.Name = "btnCetak";
            this.btnCetak.Size = new System.Drawing.Size(240, 50);
            this.btnCetak.TabIndex = 5;
            this.btnCetak.Text = "📄 Cetak Laporan";
            this.btnCetak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCetak.UseVisualStyleBackColor = true;
            // 
            // btnUsers
            // 
            this.btnUsers.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUsers.FlatAppearance.BorderSize = 0;
            this.btnUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsers.ForeColor = System.Drawing.Color.White;
            this.btnUsers.Location = new System.Drawing.Point(0, 200);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(240, 50);
            this.btnUsers.TabIndex = 4;
            this.btnUsers.Text = "👥 Kelola User";
            this.btnUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUsers.UseVisualStyleBackColor = true;
            // 
            // btnPenjemputan
            // 
            this.btnPenjemputan.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPenjemputan.FlatAppearance.BorderSize = 0;
            this.btnPenjemputan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPenjemputan.ForeColor = System.Drawing.Color.White;
            this.btnPenjemputan.Location = new System.Drawing.Point(0, 150);
            this.btnPenjemputan.Name = "btnPenjemputan";
            this.btnPenjemputan.Size = new System.Drawing.Size(240, 50);
            this.btnPenjemputan.TabIndex = 3;
            this.btnPenjemputan.Text = "🚛 Jadwal Penjemputan";
            this.btnPenjemputan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPenjemputan.UseVisualStyleBackColor = true;
            // 
            // btnSampah
            // 
            this.btnSampah.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSampah.FlatAppearance.BorderSize = 0;
            this.btnSampah.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSampah.ForeColor = System.Drawing.Color.White;
            this.btnSampah.Location = new System.Drawing.Point(0, 100);
            this.btnSampah.Name = "btnSampah";
            this.btnSampah.Size = new System.Drawing.Size(240, 50);
            this.btnSampah.TabIndex = 2;
            this.btnSampah.Text = "📦 Data Sampah";
            this.btnSampah.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSampah.UseVisualStyleBackColor = true;
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(0, 623);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(240, 50);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "🚪 Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            // 
            // panelLogo
            // 
            this.panelLogo.Controls.Add(this.lblTitleSidebar);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(240, 100);
            this.panelLogo.TabIndex = 0;
            // 
            // lblTitleSidebar
            // 
            this.lblTitleSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitleSidebar.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitleSidebar.ForeColor = System.Drawing.Color.White;
            this.lblTitleSidebar.Location = new System.Drawing.Point(0, 0);
            this.lblTitleSidebar.Name = "lblTitleSidebar";
            this.lblTitleSidebar.Size = new System.Drawing.Size(240, 100);
            this.lblTitleSidebar.TabIndex = 0;
            this.lblTitleSidebar.Text = "BANK SAMPAH";
            this.lblTitleSidebar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.White;
            this.panelHeader.Controls.Add(this.lblWelcome);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(240, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(922, 60);
            this.panelHeader.TabIndex = 1;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblWelcome.Location = new System.Drawing.Point(750, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(126, 21);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome, Admin";
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(240, 60);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(922, 613);
            this.panelContent.TabIndex = 2;
            // 
            // DashboardAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 673);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelSidebar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "DashboardAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard Admin";
            this.panelSidebar.ResumeLayout(false);
            this.panelLogo.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Label lblTitleSidebar;
        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnPenjemputan;
        private System.Windows.Forms.Button btnSampah;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWelcome;
        // 3. INI PENTING: Deklarasi Variabel Tombol
        private System.Windows.Forms.Button btnCetak;
    }
}